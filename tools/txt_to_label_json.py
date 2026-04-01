"""
txt_to_label_json.py
====================
Setup 폴더의 모든 .txt 파일을 읽어 VisionSetup용 Label JSON을 일괄 생성합니다.

출력:
  - 각 txt와 같은 경로에 동일 이름의 .json 파일
  - tools/txt_to_label_json_log.txt (실행 로그)

사용법:
  py txt_to_label_json.py
  txt_to_label_json.bat  (더블클릭 실행)
"""

import datetime
import glob
import json
import os

TOOLS_DIR   = os.path.dirname(os.path.abspath(__file__))
PROJECT_ROOT = os.path.dirname(TOOLS_DIR)
SETUP_DIR   = os.path.join(PROJECT_ROOT, "exe_New", "net8.0-windows", "Setup")
LOG_PATH    = os.path.join(TOOLS_DIR, "txt_to_label_json_log.txt")


def txt_to_labels(txt_path: str) -> list:
    with open(txt_path, encoding="utf-8") as f:
        lines = [l.strip() for l in f]
    lines = [l for l in lines if l]

    labels = []
    x, y = 0.012, 0.012
    for tag, text in enumerate(lines):
        labels.append({
            "Tag": str(tag),
            "showType": 0,
            "Text": text,
            "RelativeX": round(x, 7),
            "RelativeY": round(y, 7),
            "Width": 50,
            "Height": 25,
            "BackColor": "White",
            "ForeColor": "Black",
            "FontName": "Microsoft Sans Serif",
            "FontSize": 10.0,
            "ToolList": []
        })
        x += 0.02
        if x >= 0.9:
            x = 0.012
            y = round(y + 0.03, 7)

    return labels


def main():
    log_lines = []

    def log(msg: str):
        print(msg)
        log_lines.append(msg)

    started_at = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    log(f"{'='*60}")
    log(f"  txt_to_label_json  시작: {started_at}")
    log(f"  Setup 경로: {SETUP_DIR}")
    log(f"{'='*60}")

    txt_files = sorted(glob.glob(os.path.join(SETUP_DIR, "*.txt")))

    if not txt_files:
        log(f"[경고] .txt 파일이 없습니다: {SETUP_DIR}")
    else:
        log(f"  발견된 .txt 파일: {len(txt_files)}개\n")
        ok, fail = 0, 0
        for txt_path in txt_files:
            output_path = os.path.splitext(txt_path)[0] + ".json"
            try:
                labels = txt_to_labels(txt_path)
                with open(output_path, "w", encoding="utf-8") as f:
                    json.dump(labels, f, indent=2, ensure_ascii=False)
                msg = (f"  [OK] {os.path.basename(txt_path)}"
                       f"  →  {os.path.basename(output_path)}"
                       f"  ({len(labels)}개 라벨)")
                log(msg)
                ok += 1
            except Exception as e:
                msg = f"  [실패] {os.path.basename(txt_path)}  오류: {e}"
                log(msg)
                fail += 1

        log(f"\n  완료: 성공 {ok}개 / 실패 {fail}개")

    finished_at = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    log(f"  종료: {finished_at}")
    log(f"{'='*60}")

    with open(LOG_PATH, "w", encoding="utf-8") as f:
        f.write("\n".join(log_lines) + "\n")

    print(f"\n  로그 저장: {LOG_PATH}")
    os.startfile(SETUP_DIR)


if __name__ == "__main__":
    main()
