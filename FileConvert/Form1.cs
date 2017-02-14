using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileConvert
{
    public partial class Form1 : Form
    {
        String txtSave = "itide";
        String txt = "";
        String saveC = @"C:\TideConvert";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            if (this.ofdFile.ShowDialog() == DialogResult.OK)
            {
                this.txtPath.Text = this.ofdFile.FileName; // ofdFile 경로를 Text속성에 저장
            }
        }
        
        
        private bool txtCheck()
        {
            if (this.txtPath.Text == "")
                return false;
            else
                return true;
        }

        private bool txtSaveCheck()
        {
            if (this.txtSavePath.Text == "")
                return false;
            else
                return true;
        }

        private void btnSavePath_Click(object sender, EventArgs e)
        {
            if (this.sfdFile.ShowDialog() == DialogResult.OK)
            {
                this.txtSavePath.Text = this.sfdFile.FileName; // 저장경로지정
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
                if (txtCheck() == false)
                    return;
                if (File.Exists(this.txtPath.Text)) // 지정된 경로에 파일이 존재하는지 검사
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(this.txtPath.Text, Encoding.Default))
                        {
                            txt = sr.ReadToEnd();
                            sr.Close();
                        }
                        File.AppendAllText(txtSavePath.Text, txtSave + txt, Encoding.Default);
                        MessageBox.Show("파일이 정상적으로 변환되었습니다.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }catch(IOException){
                        MessageBox.Show("저장경로를 지정해주세요", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }else{
                    MessageBox.Show("변환할 파일을 선택해주세요..", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            //File.WriteAllText(txtSavePath.Text, txtSave + txtView.Text);
        }
        

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnConvert2_Click(object sender, EventArgs e)
        {
            if (txtCheck() == false)
                return;
            if (File.Exists(this.txtPath.Text)) // 지정된 경로에 파일이 존재하는지 검사
            {
                try
                {
                    using (StreamReader sr = new StreamReader(this.txtPath.Text, Encoding.Default))
                    {
                        txt = sr.ReadToEnd();              
                    }

                    int position = txtPath.Text.LastIndexOf("\\");
                    string fileName = txtPath.Text.Substring(position + 1); //파일명+확장자 추출
                    char[] delimeters = {'.'};
                    string[] dirSplit = fileName.Split(delimeters);
                    //string saveAuto = "tide_" + fileName;      //앞에 tide를 붙일때
                    string saveAuto = dirSplit[0] +"_tide" +"." + dirSplit[1];  //스플릿으로 나눔!
                    string newDir = saveC + @"\" + saveAuto;
                    string con = txtSave + txt;


                    //File.AppendAllText(saveC + saveAuto, txtSave + txt, Encoding.Default);
                    //MessageBox.Show("값을 알아보자 : " + saveC + saveAuto, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (!Directory.Exists(saveC))
                    {
                        Directory.CreateDirectory(saveC);
                    }
                    if (File.Exists(newDir))
                    {
                        
                    }else
                    {
                        File.AppendAllText(newDir, con, Encoding.Default);
                    }
                    
                    //Byte[] info = new UTF8Encoding(true).GetBytes(txt.Insert(0, txtSave));
                    //FileStream fs = File.Open(saveC+saveAuto, FileMode.Append);
                    //fs.Write(info, 0, info.Length);
                    //fs.Close();
                    //MessageBox.Show("값을 알아보자 : " + info.ToString(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageBox.Show("파일이 [" + newDir + "] 경로로 변환되었습니다.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (IOException)
                {
                    MessageBox.Show("에러발생", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("변환할 파일을 선택해주세요..", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 버전정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 폴더열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe",saveC);            
        }
    }
}
