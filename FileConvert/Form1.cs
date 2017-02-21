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
        String txtSave = "itide1.0 ";
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
        
        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void 버전정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            version v = new version();
            v.ShowDialog();
        }

        private void 폴더열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe",saveC);            
        }


        private void btnTest_Click(object sender, EventArgs e)
        {
            
            if (txtCheck() == false)
                return;
            if (File.Exists(this.txtPath.Text)) // 지정된 경로에 파일이 존재하는지 검사
            {
                try
                {
                    //txt형식 default
                    using (StreamReader sr = new StreamReader(this.txtPath.Text, Encoding.Default))
                    {
                        txt = sr.ReadToEnd();
                        sr.Close();
                    }
                    
                    int position = txtPath.Text.LastIndexOf("\\");
                    string fileName = txtPath.Text.Substring(position + 1); 
                    char[] delimeters = { '.' };
                    string[] dirSplit = fileName.Split(delimeters);
                    string saveAuto = dirSplit[0] + "_itide" + "." + dirSplit[1];  
                    string newDir = saveC + @"\" + saveAuto;
                    string con = txtSave + txt; 

                    //bin형식unicode
                    byte[] dataTide = Encoding.Default.GetBytes("itide1.0");
                    byte[] dataRead = File.ReadAllBytes(txtPath.Text);
                    String strData = System.Text.Encoding.Unicode.GetString(dataRead);
                    String strTide = System.Text.Encoding.Unicode.GetString(dataTide);
                    String strTotal = strTide + strData;
                    byte[] byteTotal = System.Text.Encoding.Unicode.GetBytes(strTotal);  //여기까지는 itide1.0 + 원데이터


                    ushort crcCheck = CalcCRC(byteTotal); //CRC 코드(itide1.0+data)


                    String crcCode = crcCheck.ToString("X");
                    byte[] crcByte = Encoding.Default.GetBytes(crcCode);
                    String crcCode1 = Encoding.Unicode.GetString(crcByte);
                    String strFinish = strTotal + crcCode1;
                    byte[] byteFinish = Encoding.Unicode.GetBytes(strFinish); // itide1.0+data+crc



                    if (!Directory.Exists(saveC))
                    {
                        Directory.CreateDirectory(saveC);
                    }
                    if (File.Exists(newDir))
                    {
                        MessageBox.Show("변환한 파일이 이미 존재합니다. 삭제하고 다시 변환해주세요.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (dirSplit[1]=="bin")
                        {
                            File.WriteAllBytes(newDir, byteFinish);
                        }else if(dirSplit[1] == "txt")
                        {
                            File.AppendAllText(newDir, con, Encoding.Default);
                        }
                        MessageBox.Show("파일이 [" + newDir + "] 경로로 변환되었습니다.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //MessageBox.Show("CRC 코드는 [" + crcCode + "] 입니다.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);                
                    }
                            
                }
                catch (IOException ioe)
                {
                    MessageBox.Show("에러발생 : " + ioe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (AccessViolationException ave)
                {
                    MessageBox.Show("에러발생 : " + ave.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("변환할 파일을 선택해주세요..", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static ushort CalcCRC(byte[] strPacket)
        {
            ushort[] CRC16_TABLE = { 0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400 };
            ushort usCRC = 0xFFFF;
            ushort usTemp = 0;

            foreach (char cCurrent in strPacket)
            {
                byte bytCurrent = Convert.ToByte(cCurrent);// lower 4 bits 
                usTemp = CRC16_TABLE[usCRC & 0x000F];
                usCRC = (ushort)((usCRC >> 4) & 0x0FFF);
                usCRC = (ushort)(usCRC ^ usTemp ^ CRC16_TABLE[bytCurrent & 0x000F]); // Upper 4 Bits 
                usTemp = CRC16_TABLE[usCRC & 0x000F];
                usCRC = (ushort)((usCRC >> 4) & 0x0FFF);
                usCRC = (ushort)(usCRC ^ usTemp ^ CRC16_TABLE[(bytCurrent >> 4) & 0x000F]);
            }
            return usCRC;
        }

        public static string GetStringByteCollection(string StringValue, int ByteCnt)
        {
            string lsTmpStringValue = string.Empty;
            string lsReturnValue = string.Empty;
            int liByteCnt = 0;
            try
            {
                for (int liStartPoint = 0; liStartPoint < StringValue.Length; liStartPoint++)
                {
                    lsTmpStringValue = lsTmpStringValue + StringValue.Substring(liStartPoint, 1).Replace("'", "''");
                    liByteCnt = Encoding.Default.GetByteCount(lsTmpStringValue);

                    if (liByteCnt > ByteCnt)
                    {
                        lsReturnValue = lsTmpStringValue.Substring(0, lsTmpStringValue.Length - 1);
                        break;
                    }
                    else
                    {
                        lsReturnValue = lsTmpStringValue.ToString();
                    }
                }
                return lsReturnValue.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private void btnConvert2_Click(object sender, EventArgs e)
        {
            if (txtCheck() == false)
                return;
            if (File.Exists(this.txtPath.Text)) // 지정된 경로에 파일이 존재하는지 검사
            {
                try
                {
                    FileStream fs = File.OpenRead(txtPath.Text);
                    BinaryReader br = new BinaryReader(fs);        
                    
                    //정상적인지 위변조되있는지 모르는 데이터를 불러와서 itide1.0+data부분을 crc 체크       
                    //[검사CRC]   
                    byte[] dataAllRead = File.ReadAllBytes(txtPath.Text);   //itide1.0+data+crc가포함된데이터
                    int size = dataAllRead.Length-4;
                    byte[] byteSub = br.ReadBytes(size);    // itide1.0+data만 포함된 데이터 (crc 제외)
                    ushort crcCheck = CalcCRC(byteSub);
                    string crcCode = crcCheck.ToString("X"); // 변조되었는지 byteSub을 다시한번 crc 체크함

                    //itide로 변환된 데이터 하단에 마킹되있는 crc를 불러옴
                    //[기존CRC]
                    String strDataAllRead = Encoding.Unicode.GetString(dataAllRead);
                    String strCRC = strDataAllRead.Substring(dataAllRead.Length/2-2,2);
                    byte[] byteCRC = Encoding.Unicode.GetBytes(strCRC);
                    string strCRCutf = Encoding.UTF8.GetString(byteCRC);


                    if (strCRCutf == crcCode)
                    {
                        MessageBox.Show("CRC 일치!! 기존 CRC [ " + strCRCutf + " ] + 검사 CRC [ " + crcCode + " ] 정상적인 파일입니다.", "정상적인파일", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (strCRC != crcCode)
                    {
                        MessageBox.Show("CRC가 등록되어 있지 않거나, 파일이 변조되어 CRC가 일치하지 않습니다.", "불일치파일", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("에러", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    fs.Close();
                    br.Close();                    
                                               
                }
                catch (IOException ioe)
                {
                    MessageBox.Show("에러발생 : " + ioe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (AccessViolationException ave)
                {
                    MessageBox.Show("에러발생 : " + ave.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("CRC 체크를 할 파일을 선택해주세요..", "알림", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
