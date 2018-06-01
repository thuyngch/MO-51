using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace COMMUNICATION
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            getAvailablePorts();
            getBaudrate();
            getMode();
        }

        void getBaudrate()
        {
            Baud.Items.Add("9600");
            Baud.Items.Add("19200");
            Baud.Items.Add("115200");
        }

        private void getMode()
        {
            Mode.Items.Add("NL");
            Mode.Items.Add("CR");
            Mode.Items.Add("CL&CR");
        }

        int mode = 0;
        private void btCom_Click(object sender, EventArgs e)
        {
            if (cbCom.Text == "")
            {
                MessageBox.Show("ERROR\r\n" + "You must select a COM!");
            }
            else
            {
                if (lbStatus.Text == "Disconnected")
                {
                    try
                    {
                        Com.PortName = cbCom.Text;
                        Com.BaudRate = int.Parse(Baud.SelectedItem.ToString());
                        Com.Open();
                        lbStatus.Text = "Connected";
                        btCom.Text = "Disconnect";
                        cbCom.Enabled = false;
                        Baud.Enabled = false;
                        Mode.Enabled = false;

                        /* Set mode */
                        if (Mode.SelectedItem.ToString() == "NL")
                            mode = 1;
                        else if (Mode.SelectedItem.ToString() == "CR")
                            mode = 2;
                        else if (Mode.SelectedItem.ToString() == "NL&CR")
                            mode = 3;
                        else
                            mode = 0;

                        /* Try to send handshake message */
                        Com.WriteLine("\n--MO@51--\n");
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("ERROR\r\nThe COM hasn't existed yet!\r\n");
                    }

                }
                else
                {
                    try
                    {
                        Com.Close();
                        lbStatus.Text = "Disconnected";
                        btCom.Text = "Connect";
                        cbCom.Enabled = true;
                        Baud.Enabled = true;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("ERROR\r\nThe COM hasn't existed yet!\r\n");
                    }
                }
            }
        }

        private void btSend_Click(object sender, EventArgs e)
        {
            string strSend;
            if (lbStatus.Text == "Connected")
            {
                strSend = txtSend.Text;
                Com.WriteLine(strSend);
                txtSend.Clear();
            }
            else
            {
                MessageBox.Show("ERROR\r\n" + "You must connect to a COM!");
            }
        }

        private void ReadEven(object sender, SerialDataReceivedEventArgs e)
        {
            string strReceive;
            strReceive = Com.ReadExisting();
            //if (strReceive == "\n" || strReceive == "\r" || strReceive == "\r\n")
            //{
            //    switch (mode)
            //    {
            //        case 1: /*NL*/
            //            if (strReceive == "\n") strReceive = "\r\n";
            //            break;
            //        case 2: /*CR*/
            //            if (strReceive == "\r") strReceive = "\r\n";
            //            break;
            //        case 3: /*NL&CR*/
            //            if (strReceive == "\r\n") strReceive = "\r\n";
            //            break;
            //    }
            //}
            txtReceive.AppendText(strReceive);
        }

        private void Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btSend.PerformClick();
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            txtReceive.Clear();
            txtSend.Clear();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            getAvailablePorts();
        }

        int varNumOfCom = 0;
        void getAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();
            if (varNumOfCom != ports.Length)
            {
                varNumOfCom = ports.Length;
                cbCom.Text = "";
                cbCom.Items.Clear();
                cbCom.Items.AddRange(ports);
            }
        }

        private async void btnOpen_Click(object sender, EventArgs e)
        {
            /* Files */
            Stream fileHex = null;          // File Hex

            /* Select file and processing */
            if (lbStatus.Text == "Connected")
            {
                FileDialog.ShowDialog();
                if(FileDialog.CheckFileExists)
                {
                    try
                    {
                        if ((fileHex = FileDialog.OpenFile()) != null)
                        {
                            using (fileHex)
                            {
                                ///* Create preHex file */
                                //if(createFilePre(fileHex))
                                //{
                                //    /* Create binary file */
                                //    if(createFileBin())
                                //    {
                                //        /* Send to COM port */
                                //        sendCOM();
                                //    }
                                //}

                                byte[] buff = new byte[1];
                                int ui8Read = 0;
                                txtReceive.AppendText("\r\n--Downloading code into hardware--\r\n");
                                while (true)
                                {
                                    ui8Read = fileHex.ReadByte();
                                    if (ui8Read == -1) break;
                                    if (ui8Read == 13)
                                        continue;
                                    buff[0] = (byte)ui8Read;
                                    Com.Write(buff, 0, 1);
                                    await Task.Delay(20);
                                }

                                /* Close file */
                                fileHex.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("ERROR\r\n" + "You must connect to a COM!");
            }
        }

        private void sendCOM()
        {
            byte[] buff = new byte[1];
            int ui8Read = 0;
            var fileBin = new FileStream(@"C:\Users\Public\Documents\fileBin.bin", FileMode.Open);
            txtReceive.AppendText("\r\n--Downloading code into hardware--\r\n");
            while (true)
            {
                ui8Read = fileBin.ReadByte();
                if (ui8Read == -1) break;
                buff[0] = (byte)ui8Read;
                Com.Write(buff, 0, 1);
            }
        }

        unsafe private bool createFileBin()
        {
            /* Declare */
            int ui8Read = 0;
            int ui8Data = 0;
            var fileBin = new FileStream(@"C:\Users\Public\Documents\fileBin.bin", FileMode.Create);
            var filePre = new FileStream(@"C:\Users\Public\Documents\filePre.txt", FileMode.Open);
            long ui8Len = filePre.Length / 2;

            /* Starting of fileBin */
            fileBin.WriteByte((byte)1);                 // 0x01: Start of heading
            fileBin.WriteByte((byte)2);                 // 0x02: Start of text

            /* Length of data */
            if (ui8Len > 65536)
            {
                MessageBox.Show("The length of file is so large: " + ui8Len.ToString(), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                filePre.Close();
                fileBin.Close();
                return false;
            }
            fileBin.WriteByte((byte)(ui8Len >> 8));     // High byte of length
            fileBin.WriteByte((byte)ui8Len);            // Low byte of length

            /* Translate filePre to fileBin */
            while (true)
            {
                /* High nibble */
                ui8Read = filePre.ReadByte();
                if (ui8Read == -1) break;
                hexChar2Dec(ui8Read, &ui8Data); ui8Data *= 16;

                /* Low nibble */
                ui8Read = filePre.ReadByte();
                hexChar2Dec(ui8Read, &ui8Read); ui8Data += ui8Read;

                /* Write binary */
                fileBin.WriteByte((byte)ui8Data);
            }

            /* Endding of fileBin */
            fileBin.WriteByte((byte)3);                 // 0x03: End of text
            fileBin.WriteByte((byte)4);                 // 0x04: End of transmission

            /* Close */
            filePre.Close();
            fileBin.Close();
            return true;
        }

        unsafe bool createFilePre(Stream fileHex)
        {
            /* Declare */
            int ui8Read = 0;
            int ui8Count = 0;

            using (var filePre = new FileStream(@"C:\Users\Public\Documents\filePre.txt", FileMode.Create))
            {
                while (true)
                {
                    /* Get count */
                    fileHex.ReadByte();
                    ui8Read = fileHex.ReadByte();
                    hexChar2Dec(ui8Read, &ui8Count); ui8Count *= 16;
                    ui8Read = fileHex.ReadByte();
                    hexChar2Dec(ui8Read, &ui8Read); ui8Count += ui8Read;

                    /* Check EOF */
                    fileHex.ReadByte(); fileHex.ReadByte(); fileHex.ReadByte();
                    fileHex.ReadByte(); fileHex.ReadByte();
                    ui8Read = fileHex.ReadByte();
                    if (ui8Read == '1') break;

                    /* Read and write */
                    while (true)
                    {
                        /* Check length of file */
                        if (filePre.Length > 65536 * 2)
                        {
                            MessageBox.Show("The selected file is invalid!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            filePre.Close();
                            return false;
                        }

                        /* Read and write */
                        ui8Read = fileHex.ReadByte();
                        filePre.WriteByte((byte)ui8Read);
                        ui8Read = fileHex.ReadByte();
                        filePre.WriteByte((byte)ui8Read);
                        ui8Count--;
                        if (ui8Count == 0) break;
                    }

                    /* Go to new line */
                    fileHex.ReadByte(); fileHex.ReadByte(); fileHex.ReadByte();
                    fileHex.ReadByte();
                }

                /* Close */
                filePre.Close();
                return true;
            }
        }

        unsafe bool hexChar2Dec(int ui8Hex, int *ui8DesAddr)
        {
            /* If number */
            if (ui8Hex >= '0' && ui8Hex <= '9')
            {
                *ui8DesAddr = ui8Hex - '0';
                return true;
            }

            /* If upper case */
            if (ui8Hex >= 'A' && ui8Hex <= 'F')
            {
                *ui8DesAddr = ui8Hex - 'A' + 10;
                return true;
            }

            /* If lower case */
            if (ui8Hex >= 'a' && ui8Hex <= 'f')
            {
                *ui8DesAddr = ui8Hex - 'a' + 10;
                return true;
            }

            /* If being here, return false */
            return false;
        }
    }
}
