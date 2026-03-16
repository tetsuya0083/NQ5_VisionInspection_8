using Common;
using YONGSAN_CPAD_VISION;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using KEYENCE;
using System.Runtime.InteropServices;
using System.Threading;
using Vision.Shared;

namespace VisionSetup
{
    public partial class ModelSetupForm : Form
    {
        // P/Invoke declarations
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void lblTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        private PropertyWindow propertyWindow = null;

        public ModelSetupForm()
        {
            InitializeComponent();
            propertyWindow = new PropertyWindow(this);
            propertyWindow.Owner = this;
        }

        private void btnCloseModel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (propertyWindow != null && !propertyWindow.IsDisposed)
                {
                    propertyWindow.Dispose();
                }
            }
            catch { }

            this.Close();
            Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void menuTree_Opening(object sender, CancelEventArgs e)
        {
            menuModelDuplicate.Enabled = false;
            TreeNode node = treeModel.SelectedNode;
            if (node == null)
            {
                menuModelAdd.Enabled = true;
                menuModelRemove.Enabled = menuStepToolsPaste.Enabled = false;
                return;
            }

            if (node.Level == 0) // 모델을 선택한 경우 모델 추가/삭제 가능
            {
                menuModelAdd.Enabled = menuModelRemove.Enabled = true;
                menuModelDuplicate.Enabled = true;
            }
        }

        public List<Model> ModelList = new List<Model>();
        public List<CameraObject> CameraList = new List<CameraObject>();
        public string iniSetup = string.Empty;
        private void ModelSetupForm_Load(object sender, EventArgs e)
        {
            RegisterEventHandler();
            CreateCursors();

            InitValues();

            iniSetup = Util.GetWorkingDirectory() + "\\Setup" + Program.EQUIPMENT + ".ini";
            //Util.SetWindowScreen(this, Util.GetIniFileInt(iniSetup, "Setup", "ScreenNo", 2));

            //this.WindowState = FormWindowState.Maximized;
        }

        private void lblModelTree_Click(object sender, EventArgs e)
        {
            InitValues();
        }

        private void InitValues()
        {
            lblTitle.Text = Util.GetIniFileString(Program.iniSetup, "Title", "Setup", "");

            CameraList = CameraObject.GetCameraList(Program.iniCamera);
            string ini = Util.GetWorkingDirectory() + "\\MODELS.ini";
            ModelList = Model.GetModelList(ini, CameraList);

            /*
            for (int i = 0; i < ModelList.Count; i++)
            {
                Model m = ModelList[i];
                string key = $"MODEL{i + 1}";
                Util.SetIniFileString(ini, key, "ServerName", m.ModelServerName);
                Util.SetIniFileString(ini, key, "ID", m.ModelID.ToString());
                Util.SetIniFileString(ini, key, "PLCNumber", m.ModelPLCNumber.ToString());
                Util.SetIniFileString(ini, key, "Image", m.ModelImage);
            }
            */
            ShowModelTree();
        }

        private void RegisterEventHandler()
        {
            this.picImage.MouseDown += new MouseEventHandler(this.picImage_MouseDown);
            this.picImage.MouseMove += new MouseEventHandler(this.picImage_MouseMove);
            this.picImage.MouseUp += new MouseEventHandler(this.picImage_MouseUp);
            this.picImage.MouseWheel += new MouseEventHandler(picImage_MouseWheel);
        }

        public Cursor cursorZoomIn = null;
        public Cursor cursorZoomOut = null;
        public Cursor cursorPanReady = null;
        public Cursor cursorPanIng = null;
        public Cursor[] cursors = { Cursors.Default, Cursors.Default, Cursors.Default, Cursors.Default };
        private void CreateCursors()
        {
            int idx = 0;
            string pathImage = Util.GetWorkingDirectory() + "\\icons\\";
            {
                Bitmap b = new Bitmap(pathImage + "Cursor_ZoomIn.png");
                Graphics g = Graphics.FromImage(b);
                IntPtr ptr = b.GetHicon();
                cursorZoomIn = new Cursor(ptr);
                cursors[idx] = cursorZoomIn;
                idx++;
            }
            {
                Bitmap b = new Bitmap(pathImage + "Cursor_ZoomOut.png");
                Graphics g = Graphics.FromImage(b);
                IntPtr ptr = b.GetHicon();
                cursorZoomOut = new Cursor(ptr);
                cursors[idx] = cursorZoomOut;
                idx++;
            }
            {
                Bitmap b = new Bitmap(pathImage + "Cursor_PanReady.png");
                Graphics g = Graphics.FromImage(b);
                IntPtr ptr = b.GetHicon();
                cursorPanReady = new Cursor(ptr);
                cursors[idx] = cursorPanReady;
                idx++;
            }
            {
                Bitmap b = new Bitmap(pathImage + "Cursor_PanIng.png");
                Graphics g = Graphics.FromImage(b);
                IntPtr ptr = b.GetHicon();
                cursorPanIng = new Cursor(ptr);
                cursors[idx] = cursorPanIng;
                idx++;
            }
        }

        private void ShowModelTree()
        {
            treeModel.Nodes.Clear();
            for (int i = 0; i < ModelList.Count; i++)
            {
                Model mo = ModelList[i];
                _ = treeModel.Nodes.Add(mo.ModelID.ToString(), mo.ModelServerName, 0, 0);
            }

            if (treeModel.Nodes.Count > 0)
                treeModel.Nodes[0].EnsureVisible();
        }

        private void menuModelDuplicate_Click(object sender, EventArgs e)
        {
            TreeNode node = treeModel.SelectedNode;
            if (node == null)
                return;

            Model moSrc = ModelList.Find(x => x.ModelID == Int32.Parse(node.Name));

            int maxID = ModelList.Max(m => m.ModelID);
            Model mo = Model.DuplicateModel(Program.iniModel, moSrc, maxID + 1);
            if (mo == null)
                return;

            ModelList.Add(mo);

            // 트리 노드에 추가
            TreeNode nodeModel = treeModel.Nodes.Add(mo.ModelID.ToString(), mo.ModelServerName, 0, 0);

            treeModel.SelectedNode = nodeModel;
            treeModel.SelectedNode.Expand();
            UpdateAllValues();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (CurrentModel == null)
                return;

            string ini = Path.Combine(Util.GetWorkingDirectory(), "MODELS.INI");
            TreeNode node = treeModel.SelectedNode;
            if (CurrentModel != null)
            {
                // json 파일 이름 변경 
                if (CurrentModel.ModelServerName != txtServerModelName.Text)
                {
                    try
                    {
                        string fileSrc = Path.Combine(Util.GetWorkingDirectory(), "Setup", $"{CurrentModel.ModelServerName}.json");
                        string fileDst = Path.Combine(Util.GetWorkingDirectory(), "Setup", $"{txtServerModelName.Text}.json");
                        File.Move(fileSrc, fileDst);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Apply failed. " + ex.Message);
                        return;
                    }
                }

                string key = "MODEL" + CurrentModel.ModelID.ToString();
                CurrentModel.ModelServerName = txtServerModelName.Text;
                Int32.TryParse(txtPLCModelNumber.Text, out CurrentModel.ModelPLCNumber);

                Util.SetIniFileString(Program.iniModel, key, "ServerName", CurrentModel.ModelServerName);
                Util.SetIniFileString(Program.iniModel, key, "PLCNumber", CurrentModel.ModelPLCNumber.ToString());
                Util.SetIniFileString(Program.iniModel, key, "Image", CurrentModel.ModelImage);
            }
        }

        public List<Label> labelControls = new List<Label>();
        internal void DeleteLabel(object obj)
        {
            LabelModel model = CurrentModel.LabelList.FirstOrDefault(x => x.Tag == ((LabelModel)obj).Tag.ToString());
            if (model != null)
                CurrentModel.LabelList.Remove(model);

            Label lbl = labelControls.FirstOrDefault(x => x.Tag.ToString() == ((LabelModel)obj).Tag.ToString());
            if (lbl != null)
            {
                if (lbl.Parent != null)
                    lbl.Parent.Controls.Remove(lbl);   // 화면에서 제거
                lbl.Dispose();
            }

            CurrentModel.SaveInformation();
        }

        internal LabelModel DuplicateLabel(object obj)
        {
            LabelModel original = obj as LabelModel;
            if (original == null)
                return null;

            // 1) 원본 Label 찾아오기
            Label originalLbl = labelControls
                .FirstOrDefault(x => x.Tag.ToString() == original.Tag);

            if (originalLbl == null)
                return null;

            // 2) 새 Label(UI) 생성
            Label newLbl = new Label();
            newLbl.AutoSize = false;
            newLbl.Text = original.Text + "-Copy";
            newLbl.Font = new Font(original.FontName, original.FontSize);
            newLbl.BackColor = original.BackColor;
            newLbl.ForeColor = original.ForeColor;
            newLbl.BorderStyle = originalLbl.BorderStyle;
            newLbl.TextAlign = originalLbl.TextAlign;
            newLbl.Width = original.Width;
            newLbl.Height = original.Height;

            // 위치는 +10씩 이동해서 복제
            newLbl.Left = originalLbl.Left;
            newLbl.Top = (originalLbl.Top > picImage.Height / 2) ? originalLbl.Top - 30 : originalLbl.Top + 30;

            // 3) 새 LabelModel 생성
            LabelModel model = new LabelModel()
            {
                Tag = CurrentModel.LabelList.Count.ToString(),
                Text = original.Text + "-Copy",
                RelativeX = (float)newLbl.Left / picImage.Width,
                RelativeY = (float)newLbl.Top / picImage.Height,
                Width = original.Width,
                Height = original.Height,
                FontName = original.FontName,
                FontSize = original.FontSize,
                BackColor = original.BackColor,
                ForeColor = original.ForeColor,
            };

            // ToolList 복제
            foreach (var t in original.ToolList)
            {
                model.ToolList.Add(new Tool()
                {
                    ProgramNo = t.ProgramNo,
                    ToolNo = t.ToolNo,
                    CameraNo = t.CameraNo,
                    PosAdjust = t.PosAdjust,
                });
            }

            // 4) UI/Model 연결
            newLbl.Tag = model.Tag;
            newLbl.Click += Lbl_Click;
            newLbl.MouseDown += Lbl_MouseDown;

            // 5) List에 추가
            picImage.Controls.Add(newLbl);
            labelControls.Add(newLbl);
            CurrentModel.LabelList.Add(model);

            // 6) 저장
            CurrentModel.SaveInformation();

            return model;
        }

        public LabelModel AddLabelModel()
        {
            // 1) UI Label 생성
            Label lbl = new Label();
            lbl.Text = $"Label{CurrentModel.LabelList.Count}";
            lbl.AutoSize = false;
            lbl.BackColor = Color.White;
            lbl.Font = new Font("Microsoft San Serif", 8);
            lbl.BorderStyle = BorderStyle.FixedSingle;
            lbl.TextAlign = ContentAlignment.MiddleCenter;

            lbl.Left = 50;
            lbl.Top = 50;
            lbl.Width = 35;
            lbl.Height = 20;

            lbl.Click += Lbl_Click;
            lbl.MouseDown += Lbl_MouseDown;

            picImage.Controls.Add(lbl);
            labelControls.Add(lbl);

            // 2) LabelModel 생성
            LabelModel model = new LabelModel()
            {
                Tag = CurrentModel.LabelList.Count.ToString(),
                Text = lbl.Text,
                RelativeX = (float)lbl.Left / picImage.Width,
                RelativeY = (float)lbl.Top / picImage.Height,
                Width = lbl.Width,
                Height = lbl.Height,
                FontName = lbl.Font.Name,
                FontSize = lbl.Font.Size,
                BackColor = lbl.BackColor,
                ForeColor = lbl.ForeColor
            };

            CurrentModel.LabelList.Add(model);

            lbl.Tag = model.Tag;

            CurrentModel.SaveInformation();

            return model;
        }

        private void CreateLabelList(LabelModel model)
        {
            // 1) UI Label 생성
            Label lbl = new Label()
            {
                Text = model.Text,
                AutoSize = false,
                BackColor = model.BackColor,
                ForeColor = model.ForeColor,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = (int)(model.RelativeX * picImage.Width),
                Top = (int)(model.RelativeY * picImage.Height),
                Width = model.Width,
                Height = model.Height,
            };
            // Font는 이렇게 따로 설정해야 함
            lbl.Font = new Font(model.FontName, model.FontSize);
            lbl.Click += Lbl_Click;
            lbl.MouseDown += Lbl_MouseDown;

            picImage.Controls.Add(lbl);
            labelControls.Add(lbl);
            lbl.Tag = model.Tag;
        }

        Label currentDragLabel = null;
        Point dragStartMouse;     // PictureBox 기준 마우스 좌표
        Point dragStartLabelPos;  // Label 원래 위치
        private void Lbl_Click(object sender, EventArgs e)
        {
        }

        private void Lbl_MouseDown(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;

            if (lbl == null)
                return;

            LabelModel model = CurrentModel.LabelList.FirstOrDefault(x => x.Tag == lbl.Tag.ToString());
            if (model != null)
            {
                propertyWindow.SelectedObject(model);
                propertyWindow.Show();
            }

            currentDragLabel = lbl;

            dragStartMouse = picImage.PointToClient(Cursor.Position);
            dragStartLabelPos = currentDragLabel.Location;

            picImage.Capture = true;
        }

        public void Grid_PropertyValueChanged(object obj, PropertyValueChangedEventArgs e)
        {
            LabelModel model = obj as LabelModel;
            if (model == null)
                return;

            // Label 찾기
            Label lbl = labelControls.FirstOrDefault(x => x.Tag.ToString() == model.Tag.ToString());
            if (lbl == null)
                return;

            // Model → UI 적용
            lbl.Text = model.Text;
            lbl.BackColor = model.BackColor;
            lbl.ForeColor = model.ForeColor;
            lbl.Font = new Font(model.FontName, model.FontSize);

            lbl.Left = (int)(model.RelativeX * picImage.Width);
            lbl.Top = (int)(model.RelativeY * picImage.Height);

            lbl.Width = model.Width;
            lbl.Height = model.Height;

            CurrentModel.SaveInformation();
        }

        private void picImage_MouseDown(object sender, MouseEventArgs e)
        {
            /*
            foreach (var lbl in labelControls)
            {
                if (lbl.Bounds.Contains(e.Location))
                {
                    currentDragLabel = lbl;
                    dragStartMouse = e.Location;
                    dragStartLabelPos = lbl.Location;
                    break;
                }
            }
            */
        }

        private void picImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentDragLabel != null && e.Button == MouseButtons.Left)
            {
                int dx = e.X - dragStartMouse.X;
                int dy = e.Y - dragStartMouse.Y;

                currentDragLabel.Left = dragStartLabelPos.X + dx;
                currentDragLabel.Top = dragStartLabelPos.Y + dy;

                // 모델도 즉시 반영
                LabelModel model = CurrentModel.LabelList.FirstOrDefault(x => x.Tag == currentDragLabel.Tag.ToString());
                if (model != null)
                {
                    model.RelativeX = (float)currentDragLabel.Left / picImage.Width;
                    model.RelativeY = (float)currentDragLabel.Top / picImage.Height;
                }

                propertyWindow.RefreshGrid();
            }
        }

        private void picImage_MouseUp(object sender, MouseEventArgs e)
        {
            if (CurrentModel == null)
                return;

            currentDragLabel = null;
            picImage.Capture = false;
            CurrentModel.SaveInformation();
        }

        private void RemoveLabelModel(int id)
        {
        }

        public void ClearLabels()
        {
            foreach (var lbl in labelControls)
            {
                if (lbl.Parent != null)
                    lbl.Parent.Controls.Remove(lbl);   // 화면에서 제거
                lbl.Dispose();
            }

            labelControls.Clear();    // 리스트 비우기
        }

        private void menuModelAdd_Click(object sender, EventArgs e)
        {
            int MaxID = Util.GetIniFileInt(Program.iniModel, "Model", "MaxID", 0);
            int NewID = MaxID + 1;

            Model mo = new Model();
            mo.ModelID = NewID;
            mo.ModelServerName = "MODEL" + NewID.ToString();
            mo.ModelPLCNumber = NewID;
            mo.ModelImage = string.Empty;
            mo.CameraList = CameraList;
            ModelList.Add(mo);

            TreeNode node = treeModel.Nodes.Add(NewID.ToString(), mo.ModelServerName, 0);
            UpdateAllValues();
            node.Expand();
            treeModel.SelectedNode = node;

            string category = "MODEL" + mo.ModelID.ToString();
            Util.SetIniFileString(Program.iniModel, category, "ID", NewID.ToString());
            Util.SetIniFileString(Program.iniModel, category, "ServerName", mo.ModelServerName);
            Util.SetIniFileString(Program.iniModel, category, "PLCNumber", mo.ModelPLCNumber.ToString());
            Util.SetIniFileString(Program.iniModel, category, "Image", string.Empty);

            // MaxID 증가
            Util.SetIniFileString(Program.iniModel, "Model", "MaxID", NewID.ToString());
            UpdateAllValues();
        }

        private void menuModelRemove_Click(object sender, EventArgs e)
        {
            TreeNode node = treeModel.SelectedNode;
            if (node == null)
                return;

            int id = Int32.Parse(node.Name);
            string key = "MODEL" + id.ToString();
            Util.SetIniFileNull(Program.iniModel, key);
            for (int i = 0; i < ModelList.Count; i++)
            {
                Model mo = ModelList[i];
                if (mo.ModelID == id)
                {
                    Model.Remove(Program.iniModel, mo);

                    ModelList.Remove(mo);
                    break;
                }
            }
            node.Remove();
            UpdateAllValues();
        }

        private void treeModel_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateAllValues();
        }

        public Model CurrentModel = null;

        private void UpdateAllValues()
        {
            ClearLabels();
            CurrentModel = null;

            TreeNode node = treeModel.SelectedNode;
            if (node == null)
            {
                txtServerModelName.Text = txtPLCModelNumber.Text = string.Empty;
                btnChangeImage.Enabled = btnApply.Enabled = false;
                picImage.Image = null;
                return;
            }

            btnChangeImage.Enabled = btnApply.Enabled = true;
            CurrentModel = ModelList.Find(x => x.ModelID == Int32.Parse(node.Name));

            txtServerModelName.Text = CurrentModel.ModelServerName;
            txtPLCModelNumber.Text = CurrentModel.ModelPLCNumber.ToString();

            string full = Util.GetWorkingDirectory() + "\\Images\\" + CurrentModel.ModelImage;
            picImage.Image = Util.GetBitmapFromFile(full);

            for (int i = 0; i < CurrentModel.LabelList.Count; i++)
            {
                CreateLabelList(CurrentModel.LabelList[i]);
            }
            propertyWindow.SelectedObject(null);
            propertyWindow.Show();
        }

        private Point mouseDownLocation;
        private void picTools_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 마우스 클릭 위치 저장
                mouseDownLocation = e.Location;
            }
        }

        public bool picToolMoving = false;
        private void picTools_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                picToolMoving = true;
                // PictureBox 위치 업데이트
                ((PictureBox)sender).Left = e.X + ((PictureBox)sender).Left - mouseDownLocation.X;
                ((PictureBox)sender).Top = e.Y + ((PictureBox)sender).Top - mouseDownLocation.Y;

                if (e.X - mouseDownLocation.X > 200)
                    Thread.Sleep(0);
            }
        }

        private void picTools_MouseUp(object sender, MouseEventArgs e)
        {
            /*
            if (e.Button == MouseButtons.Left)
            {
                picToolMoving = false;
                // PictureBox 위치 업데이트
                int xin = ((PictureBox)sender).Left = e.X + ((PictureBox)sender).Left - mouseDownLocation.X;
                int yin = ((PictureBox)sender).Top = e.Y + ((PictureBox)sender).Top - mouseDownLocation.Y;

                if (Int32.TryParse((string)((PictureBox)sender).Tag, out int index))
                {
                    GetScaledCoordinateStretched(xin, yin, out int xout, out int yout);
                    VisionTool vt = CurrentStep.VisionToolList[index - 1];
                    vt.PositionX = xout;
                    vt.PositionY = yout;

                    string keyStep = Step.GetStepKeyName(CurrentModel.ModelID, CurrentStep.CameraID, CurrentStep.StepNo);
                    string keyToolX = "TOOL" + index.ToString() + "X";
                    Util.SetIniFileString(Program.iniModel, keyStep, keyToolX, vt.PositionX.ToString());
                    string keyToolY = "TOOL" + index.ToString() + "Y";
                    Util.SetIniFileString(Program.iniModel, keyStep, keyToolY, vt.PositionY.ToString());
                }
            }
            */
        }

        private void picTools_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            string index = (string)pic.Tag;
            // 그리기 작업을 위한 그래픽 객체 가져오기
            Graphics g = e.Graphics;

            int fontSize = Util.GetIniFileInt(iniSetup, "PictureBox", "FontSize", 16);
            if (index.Length > 1)
                fontSize = (int)(fontSize * 0.9);

            // 글자와 폰트 설정
            Font font = new Font("Arial", fontSize, FontStyle.Bold);
            Brush brush = Brushes.White;

            Rectangle rc = new Rectangle(0, 0, pic.Width, pic.Height);
            g.FillRectangle(Brushes.Blue, rc);
            // 글자 그리기
            StringFormat format = new StringFormat
            {
                Alignment = StringAlignment.Center,  // 좌우 중앙 정렬
                LineAlignment = StringAlignment.Center // 상하 중앙 정렬
            };
            g.DrawString(index, font, brush, rc, format);
        }

        private void btnChangeImage_Click(object sender, EventArgs e)
        {
            if (CurrentModel == null)
                return;

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ofd.Filter = string.Empty;
            string sep = string.Empty;
            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                ofd.Filter = String.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }
            ofd.Filter = String.Format("{0}{1}{2} ({3})|{3}", ofd.Filter, sep, "All Files", "*.*");
            ofd.DefaultExt = ".png"; // Default file extension 
            ofd.FileName = "*";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string fileName = Path.GetFileName(ofd.FileName);
            string dst = Util.GetWorkingDirectory() + "\\Images\\" + fileName;
            try
            {
                if (ofd.FileName != dst)
                {
                    File.Delete(dst);
                    File.Copy(ofd.FileName, dst, true);
                }

                CurrentModel.ModelImage = fileName;
                picImage.Image = Util.GetBitmapFromFile(dst);
                picImage.Visible = true;
                picImage.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            if (CurrentModel == null)
                return;

            CurrentModel.ModelImage = string.Empty;
            picImage.Image = null;
            Invalidate();
        }

        public int PicturePannedLastX = 0;
        public int PicturePannedLastY = 0;
        public double PictureZoomed = 1.0;
        public double PictureRatioX = 1.0;
        public double PictureRatioY = 1.0;
        private void DrawPictureImage(Graphics g, Bitmap bm)
        {
            if (bm == null)
                return;

            int widthControl = picImage.Width;
            int heightControl = picImage.Height;
            int widthBitmap = bm.Width;
            int heightBitmap = bm.Height;

            // left, right
            int left = (int)((widthControl / 2.0) - (widthBitmap * PictureRatioX / 2.0));
            if (left < 0)
                left = 0;
            int right = (int)((widthControl / 2.0) + (widthBitmap * PictureRatioX / 2.0));
            if (right > widthControl)
                right = widthControl;
            // top, bottom
            int top = (int)((heightControl * PictureRatioY / 2.0) - (heightBitmap / 2.0));
            if (top < 0)
                top = 0;
            int bottom = (int)((heightControl * PictureRatioY / 2.0) + (heightBitmap / 2.0));
            if (bottom > heightControl)
                bottom = heightControl;

            //Rectangle rcDst = new Rectangle(left, top, (right - left), (bottom - top));
            Rectangle rcDst = new Rectangle(0, 0, picImage.Width, picImage.Height);

            Rectangle rcSrc = new Rectangle(0, 0, 0, 0);
            rcSrc.X = PicturePannedLastX;
            rcSrc.Y = PicturePannedLastY;
            rcSrc.Width = (int)(bm.Width * 1.0 / PictureZoomed);
            rcSrc.Height = (int)(bm.Height * 1.0 / PictureZoomed);

            g.DrawImage(bm, rcDst, rcSrc, GraphicsUnit.Pixel);
        }

        // 처음 클릭했을 때 실제 이미지 상에서의 좌표
        public Point ptStartCalibrated = new Point(0, 0);
        // 마우스를 움직이고 있을 때 이미지 상에서의 좌표
        public Point ptMouseMovingCalibrated = new Point(0, 0);
        // Panning 중인지?
        public bool IsPanning = false;
        // 객체를 잡고 움직이고 있는가?
        public bool IsObjectSelected = false;
        public Cursor cursorPrevious = Cursors.Default;
        public int previousDx = 0;
        public int previousDy = 0;
        public enum MOUSE_MODE { MOUSE_POINTER, MOUSE_ZOOMIN, MOUSE_ZOOMOUT, MOUSE_PAN };
        public MOUSE_MODE Mouse_Mode = MOUSE_MODE.MOUSE_POINTER;

        private void picImage_MouseWheel(object sender, MouseEventArgs e)
        {
            /*
            if (ImageBitmapOriginal == null)
                return;

            double ratio = Math.Min(picImage.Width * 1.0 / ImageBitmapOriginal.Width, picImage.Height * 1.0 / ImageBitmapOriginal.Height);

            int xmin = ((int)Math.Round(picImage.Width * 1.0 / 2.0)) - ((int)Math.Round(ImageBitmapOriginal.Width * ratio / 2.0));
            int xmax = xmin + ((int)Math.Round(ImageBitmapOriginal.Width * ratio));
            int ymin = ((int)Math.Round(picImage.Height * 1.0 / 2.0)) - ((int)Math.Round(ImageBitmapOriginal.Height * ratio / 2.0));
            int ymax = ymin + ((int)Math.Round(ImageBitmapOriginal.Height * ratio));
            // 현재 마우스 위치가 그림 밖이면 아무 동작도 하지 않음
            if (e.X <= xmin || e.X >= xmax || e.Y <= ymin || e.Y >= ymax)
                return;

            double ptRatioX = (e.X - xmin) * 1.0 / picImage.Width;
            double ptRatioY = (e.Y - ymin) * 1.0 / picImage.Height;

            GetScaledCoordinate(e.X, e.Y, out int xout, out int yout);

            // 현재 좌표가 동일 위치에 있도록 
            if (e.Delta > 0)
            {
                if (PictureZoomed < 10)
                {
                    PictureZoomed += 0.5;

                    // (ImageBitmapOriginal.Width - (ImageBitmapOriginal.Width * 1.0 / PictureZoomed)) * 1.0 / 2.0===> 좌우 여백 크기
                    //PicturePannedLastX = (int)(ImageBitmapOriginal.Width - (ImageBitmapOriginal.Width * 1.0 / PictureZoomed) * 1.0 * ptRatioX);
                    //PicturePannedLastY = (int)(ImageBitmapOriginal.Height - (ImageBitmapOriginal.Height * 1.0 / PictureZoomed) * 1.0 * ptRatioY);
                    PicturePannedLastX = (int)((ImageBitmapOriginal.Width - (ImageBitmapOriginal.Width / PictureZoomed)) / 2.0);
                    PicturePannedLastY = (int)((ImageBitmapOriginal.Height - (ImageBitmapOriginal.Height / PictureZoomed)) / 2.0);
                }
            }
            else
            {
                if (PictureZoomed > 1)
                {
                    PictureZoomed -= 0.5;
                    PicturePannedLastX = (int)((ImageBitmapOriginal.Width - (ImageBitmapOriginal.Width / PictureZoomed)) / 2.0);
                    PicturePannedLastY = (int)((ImageBitmapOriginal.Height - (ImageBitmapOriginal.Height / PictureZoomed)) / 2.0);
                }

                if (PictureZoomed == 1)
                {
                    PicturePannedLastX = 0;
                    PicturePannedLastY = 0;
                }
            }

            CheckPictureEdgeAndPanPicture(0, 0);

            picImage.Invalidate();
            picImage.Focus();
            */
        }

        private void CheckPictureEdgeAndPanPicture(int prevX, int prevY)
        {
            /*
            if ((PicturePannedLastX) <= 0)
            {
                // 왼쪽 끝이 보이면 더 이상 움직이지 않음
                PicturePannedLastX = 0;
            }
            else
            {
                int rightEnd = (int)(PicturePannedLastX + ImageBitmapOriginal.Width * 1.0 / PictureZoomed);
                if (rightEnd >= ImageBitmapOriginal.Width)
                {
                    // 오른쪽 끝이 보이면 더 이상 움직이지 않음
                    PicturePannedLastX = ImageBitmapOriginal.Width - (int)(ImageBitmapOriginal.Width * 1.0 / PictureZoomed);
                }
            }

            if ((PicturePannedLastY) <= 0)
            {
                // 위쪽 끝이 보임이면 더 이상 움직이지 않음
                PicturePannedLastY = 0;
            }
            else
            {
                int bottomEnd = (int)(PicturePannedLastY + ImageBitmapOriginal.Height * 1.0 / PictureZoomed);
                if (bottomEnd >= ImageBitmapOriginal.Height)
                {
                    // 아래쪽 끝이 보이면 더 이상 움직이지 않음
                    PicturePannedLastY = ImageBitmapOriginal.Height - (int)(ImageBitmapOriginal.Height * 1.0 / PictureZoomed);
                }
            }
            */
        }

        private void btnSystemSetup_Click(object sender, EventArgs e)
        {
            MainForm form = new MainForm();
            form.ShowDialog();

            InitValues();
        }
    }
}
