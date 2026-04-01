"""
generate_label_json.py
======================
라벨이 표시된 이미지에서 VisionSetup용 Label JSON을 자동 생성합니다.

처리 방식:
  1. 이미지 전체에 Tesseract OCR 적용 → 단어 bounding box 추출
  2. 인접한 단어들을 그룹핑 → 라벨 박스 생성
  3. 각 라벨 박스에서 텍스트 합치기 + ForeColor 추출
  4. VisionSetup LabelModel JSON 형식으로 저장

필요 라이브러리:
    pip install opencv-python pillow numpy pytesseract

Tesseract OCR 설치 (Windows):
    https://github.com/UB-Mannheim/tesseract/wiki
    설치 경로: C:\\Program Files\\Tesseract-OCR\\

사용법:
    py generate_label_json.py
    py generate_label_json.py --image after.png --output result.json
    py generate_label_json.py --debug
    py generate_label_json.py --min-conf 30 --line-gap 20 --word-gap 60
"""

import argparse
import json
import os

import cv2
import numpy as np

# Tesseract 경로 (필요시 수정)
TESSERACT_CMD = r"C:\Program Files\Tesseract-OCR\tesseract.exe"

# 기본 경로
PROJECT_ROOT  = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DEFAULT_IMAGE  = os.path.join(PROJECT_ROOT, "exe_New", "net8.0-windows", "Images", "Add Label", "after_label.png")
DEFAULT_OUTPUT = os.path.join(PROJECT_ROOT, "exe_New", "net8.0-windows", "Setup", "generated_labels.json")

# LabelModel 기본값
DEFAULT_FONT_NAME = "Microsoft Sans Serif"
DEFAULT_FONT_SIZE = 20.0
DEFAULT_SHOW_TYPE = 0  # AlwaysShow


def load_image(path: str) -> np.ndarray:
    img = cv2.imdecode(np.fromfile(path, dtype=np.uint8), cv2.IMREAD_COLOR)
    if img is None:
        raise FileNotFoundError(f"이미지 로드 실패: {path}")
    return img


def _run_ocr(gray: np.ndarray, psm: int, scale: float, min_conf: int) -> list:
    """
    전처리된 그레이스케일 이미지에 Tesseract를 실행하여 단어 박스를 반환.
    scale: 이미지를 업스케일한 배율 (좌표를 원본으로 되돌리는 데 사용)
    Returns: [(x, y, w, h, text), ...] 원본 좌표 기준
    """
    import pytesseract
    config = (f"--psm {psm} "
              "-c tessedit_char_whitelist="
              "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.")
    data = pytesseract.image_to_data(gray, output_type=pytesseract.Output.DICT, config=config)
    results = []
    for i in range(len(data["text"])):
        txt = data["text"][i].strip()
        conf = int(data["conf"][i])
        if conf >= min_conf and txt:
            bx = int(data["left"][i]   / scale)
            by = int(data["top"][i]    / scale)
            bw = int(data["width"][i]  / scale)
            bh = int(data["height"][i] / scale)
            results.append((bx, by, bw, bh, txt))
    return results


def _iou(a, b) -> float:
    """두 박스 (x,y,w,h,...) 의 IoU"""
    ax1, ay1, ax2, ay2 = a[0], a[1], a[0]+a[2], a[1]+a[3]
    bx1, by1, bx2, by2 = b[0], b[1], b[0]+b[2], b[1]+b[3]
    ix1, iy1 = max(ax1, bx1), max(ay1, by1)
    ix2, iy2 = min(ax2, bx2), min(ay2, by2)
    inter = max(0, ix2-ix1) * max(0, iy2-iy1)
    if inter == 0:
        return 0.0
    union = (ax2-ax1)*(ay2-ay1) + (bx2-bx1)*(by2-by1) - inter
    return inter / union if union > 0 else 0.0


def ocr_word_boxes(img: np.ndarray, min_conf: int = 50) -> list:
    """
    다중 패스(전처리 + PSM 조합)로 이미지 전체에서 단어 bounding box 추출.

    패스1: 그레이스케일 + CLAHE 대비 향상, psm 11 (sparse text)
    패스2: 그레이스케일 + 2배 업스케일,   psm 11
    패스3: 그레이스케일 + CLAHE,          psm 6  (uniform block)

    결과를 병합하고 IoU > 0.5 중복 제거.
    Returns: [(x, y, w, h, text), ...] 원본 좌표 기준
    """
    try:
        import pytesseract
        pytesseract.pytesseract.tesseract_cmd = TESSERACT_CMD

        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

        # CLAHE 대비 향상
        clahe = cv2.createCLAHE(clipLimit=2.0, tileGridSize=(8, 8))
        enhanced = clahe.apply(gray)

        # 2배 업스케일
        h, w = gray.shape
        upscaled = cv2.resize(enhanced, (w * 2, h * 2), interpolation=cv2.INTER_CUBIC)

        passes = [
            (enhanced, 11, 1.0),   # 패스1: CLAHE + psm 11
            (upscaled,  11, 2.0),  # 패스2: 업스케일 + psm 11
            (enhanced,   6, 1.0),  # 패스3: CLAHE + psm 6
        ]

        all_results = []
        for proc_img, psm, scale in passes:
            found = _run_ocr(proc_img, psm, scale, min_conf)
            all_results.extend(found)

        # 중복 제거: IoU > 0.5 이면 신뢰도 높은 쪽(텍스트 길이 우선) 유지
        unique = []
        for candidate in all_results:
            overlaps = False
            for kept in unique:
                if _iou(candidate, kept) > 0.5:
                    overlaps = True
                    break
            if not overlaps:
                unique.append(candidate)

        return unique

    except Exception as e:
        print(f"[경고] Tesseract 오류: {e}")
        return []


def group_words_into_labels(word_boxes: list, line_gap: int = 20, word_gap: int = 60,
                             pad: int = 5, img_w: int = 1, img_h: int = 1) -> list:
    """
    단어 bounding box 목록을 인접 기준으로 그룹핑하여 라벨 박스 생성.

    알고리즘:
      1. y 중심 기준 정렬
      2. y 차이 < line_gap → 같은 행
      3. 행 내에서 x 좌표 정렬, 단어 사이 x 간격 < word_gap → 같은 그룹

    Returns: [(x, y, w, h, text), ...]
    """
    if not word_boxes:
        return []

    # y 중심 기준 정렬
    sorted_words = sorted(word_boxes, key=lambda b: b[1] + b[3] // 2)

    # 행 분리
    lines = []
    current_line = [sorted_words[0]]
    for word in sorted_words[1:]:
        cy_prev = current_line[-1][1] + current_line[-1][3] // 2
        cy_curr = word[1] + word[3] // 2
        if abs(cy_curr - cy_prev) <= line_gap:
            current_line.append(word)
        else:
            lines.append(current_line)
            current_line = [word]
    lines.append(current_line)

    # 행 내 단어 그룹핑
    label_boxes = []
    for line in lines:
        line_sorted = sorted(line, key=lambda b: b[0])
        groups = [[line_sorted[0]]]
        for word in line_sorted[1:]:
            prev = groups[-1][-1]
            prev_right = prev[0] + prev[2]
            curr_left = word[0]
            if curr_left - prev_right <= word_gap:
                groups[-1].append(word)
            else:
                groups.append([word])

        for group in groups:
            xs  = [b[0]        for b in group]
            ys  = [b[1]        for b in group]
            x2s = [b[0] + b[2] for b in group]
            y2s = [b[1] + b[3] for b in group]
            gx  = max(0,     min(xs)  - pad)
            gy  = max(0,     min(ys)  - pad)
            gx2 = min(img_w, max(x2s) + pad)
            gy2 = min(img_h, max(y2s) + pad)
            text = " ".join(b[4] for b in group).strip()
            label_boxes.append((gx, gy, gx2 - gx, gy2 - gy, text))

    return label_boxes


def bgr_to_color_name(b: int, g: int, r: int) -> str:
    if r > 200 and g > 200 and b > 200:
        return "White"
    if r < 50 and g < 50 and b < 50:
        return "Black"
    if r > 150 and g < 100 and b < 100:
        return "Red"
    if g > 150 and r < 100 and b < 100:
        return "Green"
    if b > 150 and r < 100 and g < 100:
        return "Blue"
    if r > 180 and g > 100 and b < 80:
        return "Orange"
    if r > 150 and g > 150 and b < 80:
        return "Yellow"
    return "buttontext"


def sample_fore_color(roi: np.ndarray) -> str:
    """ROI 내 채도 높은 픽셀 → 글자색"""
    hsv = cv2.cvtColor(roi, cv2.COLOR_BGR2HSV)
    high_s = roi[hsv[:, :, 1] > 100]
    if len(high_s) == 0:
        high_s = roi[np.any(roi < 200, axis=2)]
    if len(high_s) == 0:
        return "buttontext"
    avg = np.median(high_s, axis=0).astype(int)
    return bgr_to_color_name(int(avg[0]), int(avg[1]), int(avg[2]))


def build_label_model(tag: int, text: str, x: int, y: int, w: int, h: int,
                      img_w: int, img_h: int, fore_color: str) -> dict:
    return {
        "Tag": str(tag),
        "showType": DEFAULT_SHOW_TYPE,
        "Text": text if text else f"Label{tag}",
        "RelativeX": round(x / img_w, 7),
        "RelativeY": round(y / img_h, 7),
        "Width": w,
        "Height": h,
        "BackColor": "White",
        "ForeColor": fore_color,
        "FontName": DEFAULT_FONT_NAME,
        "FontSize": DEFAULT_FONT_SIZE,
        "ToolList": []
    }


def generate(image_path: str, output_path: str,
             min_conf: int = 50, line_gap: int = 20,
             word_gap: int = 60, pad: int = 5, debug: bool = False):

    print(f"[1/4] 이미지 로드: {image_path}")
    img = load_image(image_path)
    img_h, img_w = img.shape[:2]
    print(f"      해상도: {img_w}x{img_h}")

    print(f"[2/4] Tesseract 전체 이미지 텍스트 감지 (min_conf={min_conf})")
    word_boxes = ocr_word_boxes(img, min_conf)
    print(f"      감지된 단어: {len(word_boxes)}개")
    for (wx, wy, ww, wh, wtxt) in sorted(word_boxes, key=lambda x: (x[1], x[0])):
        print(f"        ({wx:4d},{wy:4d}) {ww:3d}x{wh:2d}  \"{wtxt}\"")

    print(f"[3/4] 단어 그룹핑 → 라벨 박스 생성 (line_gap={line_gap}, word_gap={word_gap}, pad={pad})")
    label_boxes = group_words_into_labels(word_boxes, line_gap, word_gap, pad, img_w, img_h)
    print(f"      생성된 라벨 박스: {len(label_boxes)}개")

    if debug:
        debug_dir = os.path.dirname(output_path)
        dbg = img.copy()
        for (bx, by, bw, bh, _) in label_boxes:
            cv2.rectangle(dbg, (bx, by), (bx + bw, by + bh), (0, 0, 255), 3)
        box_path = os.path.join(debug_dir, "debug_boxes.png")
        cv2.imencode(".png", dbg)[1].tofile(box_path)
        print(f"      디버그: {box_path}")

    print(f"[4/4] 각 박스 색상 추출 및 JSON 저장")
    labels = []
    for i, (bx, by, bw, bh, text) in enumerate(label_boxes):
        roi = img[by:by + bh, bx:bx + bw]
        fore_color = sample_fore_color(roi)
        label = build_label_model(i, text, bx, by, bw, bh, img_w, img_h, fore_color)
        labels.append(label)
        print(f"      [{i:2d}] ({bx:4d},{by:4d}) {bw:3d}x{bh:2d}  "
              f"text='{text}'  fore={fore_color}  "
              f"RelX={label['RelativeX']:.4f} RelY={label['RelativeY']:.4f}")

    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    with open(output_path, "w", encoding="utf-8") as f:
        json.dump(labels, f, indent=2, ensure_ascii=False)
    print(f"      → {output_path}")
    print(f"      총 {len(labels)}개 라벨 생성")
    print(f"      [주의] OCR 결과는 수동 확인 후 수정하세요.")
    return labels


def main():
    parser = argparse.ArgumentParser(
        description="라벨 이미지에서 VisionSetup Label JSON 자동 생성"
    )
    parser.add_argument("--image",    default=DEFAULT_IMAGE,  help="라벨이 표시된 이미지 경로")
    parser.add_argument("--output",   default=DEFAULT_OUTPUT, help="출력 JSON 경로")
    parser.add_argument("--min-conf", type=int, default=50,   help="OCR 최소 신뢰도 0~100 (기본 50)")
    parser.add_argument("--line-gap", type=int, default=20,   help="같은 행으로 판단하는 y 거리 px (기본 20)")
    parser.add_argument("--word-gap", type=int, default=60,   help="같은 라벨로 판단하는 단어 간 x 간격 px (기본 60)")
    parser.add_argument("--pad",      type=int, default=5,    help="라벨 박스 외곽 여백 px (기본 5)")
    parser.add_argument("--debug",    action="store_true",    help="debug_boxes.png 저장")
    args = parser.parse_args()

    generate(
        image_path=args.image,
        output_path=args.output,
        min_conf=args.min_conf,
        line_gap=args.line_gap,
        word_gap=args.word_gap,
        pad=args.pad,
        debug=args.debug,
    )


if __name__ == "__main__":
    main()
