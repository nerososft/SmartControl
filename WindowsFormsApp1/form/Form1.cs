using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using uPLibrary.Networking.M2Mqtt;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;
using WindowsFormsApp1.constant;
using WindowsFormsApp1.utils;
using WindowsFormsApp1.dto;

namespace WindowsFormsApp1
{

    public delegate void SetVisiableHandler();
    public delegate void DataSendHandler(string data);
    public delegate void MessageHandler();

    public delegate void SetLED(ControlEntity control);
    public delegate void SetGraph(SaveDataDto data);

    public delegate void RemoteRun();

    public partial class Form1 : Form
    {

        int buffer = 100;
        int grid = 80;
        private bool Listening = false;//是否没有执行完invoke相关操作  
        private bool Closing = false;//是否正在关闭串口，执行Application.DoEvents，并阻止再次
        List<string> line_t_data = new List<string>();
        List<string> line_l_data = new List<string>();
        Bitmap bitmap;
        Graphics g;

        Debug debug;
        StreamView streamView;
        About aboutView;

        ControlEntity con;
        string VERSION = "1.6.6";

        String BROKER_IP = "120.27.115.59";

        MqttClient mqttClient;

        public Form1()
        {
            InitializeComponent();
        }
        private Point mPoint = new Point();
        private Point mrPoint = new Point();
        private Point mgPoint = new Point();
        private Point mbPoint = new Point();

        private void mm_down(object sender, MouseEventArgs e)
        {
            mPoint.X = e.X;
            mPoint.Y = e.Y;
        }
        private void mm_r_down(object sender, MouseEventArgs e)
        {
            mrPoint.X = e.X;
            mrPoint.Y = e.Y;
        }
        private void mm_g_down(object sender, MouseEventArgs e)
        {
            mgPoint.X = e.X;
            mgPoint.Y = e.Y;
        }
        private void mm_b_down(object sender, MouseEventArgs e)
        {
            mbPoint.X = e.X;
            mbPoint.Y = e.Y;
        }

        private void mm_move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point myPosittion = MousePosition;
                myPosittion.Offset(-mPoint.X, -mPoint.Y);
                Location = myPosittion;
                debug.Location = new Point(this.Location.X, this.Location.Y + this.Height);
                streamView.Location = new Point(this.Location.X+this.Width, this.Location.Y);
                aboutView.Location = new Point(this.Location.X + 20,this.Location.Y + 20);
            }
        }


        byte tr_r = 0;
        byte tr_g = 0;
        byte tr_b = 0;
        private void track_r_move(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X - mrPoint.X + this.track_index_r.Left+track_index_r.Width< track_r.Left + track_r.Width && e.X - mrPoint.X + this.track_index_r.Left>track_r.Left) {
                    track_index_r.Left = this.track_index_r.Left = e.X - mrPoint.X + this.track_index_r.Left;
                    int fenzi = track_index_r.Left - track_r.Left;
                    int fenmu = track_r.Width - track_index_r.Width;
                  
                    tr_r = Convert.ToByte(Math.Ceiling(fenzi*2.55));
                    track_index_r.BackColor = Color.FromArgb(255, tr_r, 0, 0);
                    pictureBox7.BackColor = Color.FromArgb(255, tr_r,tr_g,tr_b);
                }

            
               
             
                
            }
        }
        private void track_g_move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X - mgPoint.X + this.track_index_g.Left + track_index_g.Width < track_g.Left + track_g.Width && e.X - mgPoint.X + this.track_index_g.Left > track_g.Left)
                {
                    track_index_g.Left = this.track_index_g.Left = e.X - mgPoint.X + this.track_index_g.Left;

                    int fenzi = track_index_g.Left - track_g.Left;
                    int fenmu = track_g.Width - track_index_g.Width;
                    tr_g = Convert.ToByte(Math.Ceiling(fenzi * 2.55));
                    track_index_g.BackColor = Color.FromArgb(255, 0, tr_g, 0);
                    pictureBox7.BackColor = Color.FromArgb(255, tr_r, tr_g, tr_b);
              
                }
               
            }
        }
        private void track_b_move(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.X - mbPoint.X + this.track_index_b.Left + track_index_b.Width < track_b.Left + track_b.Width && e.X - mbPoint.X + this.track_index_b.Left > track_b.Left)
                {
                    track_index_b.Left = this.track_index_b.Left = e.X - mbPoint.X + this.track_index_b.Left;
                    int fenzi = track_index_b.Left - track_b.Left;
                    int fenmu = track_b.Width - track_index_b.Width;
                    tr_b = Convert.ToByte(Math.Ceiling(fenzi * 2.55));
                    track_index_b.BackColor = Color.FromArgb(255, 0, 0, tr_b);
                    pictureBox7.BackColor = Color.FromArgb(255, tr_r, tr_g, tr_b);
          
                }
             
            }
        }





        private void button1_Click_1(object sender, EventArgs e)
        {
            this.InitialSerialPort();
        }

        public void changeRGB(string cc) {
           try
            {
                if (port != null)
                {
                    //ssageBox.Show("gg");
                    port.Write(cc);
                
                    this.streamView.richTextBox2.Text =cc;
                }
            }
            catch (Exception ex)
            {
                showMessagge(false, "读取串口数据发生错误：" + ex.Message);
                //label3.Text = "读取串口数据发生错误：" + ex.Message;
            }
           
        }


       
        private SerialPort port = null;
        /// <summary>
        /// 初始化串口实例
        /// </summary>
        private void InitialSerialPort()
        {
            try
            {
                string portName = this.comboBox1.SelectedItem.ToString();
                port = new SerialPort(portName, int.Parse(this.comboBox2.SelectedItem.ToString()));
                port.Encoding = Encoding.ASCII;
                port.DataReceived += port_DataReceived;
                port.Open();
                this.ChangeArduinoSendStatus(true);
                this.label13.Text = "运行中";
                this.label13.ForeColor = Color.Green;

                this.pictureBox2.Image = this.port_start_pic.Image;
            }
            catch (Exception ex)
            {
                showMessagge(false, "初始化串口发生错误：" + ex.Message);
                //label3.Text = "初始化串口发生错误：" + ex.Message;
            }
        }
        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.RefreshInfoTextBox();
        }

        private void RefreshInfoTextBox()
        {
            if (Closing) return;
            string value = this.ReadSerialData();

          
           
            Action<string> setDataReceived = text => this.streamView.richTextBox1.Text = text;

            Action<string> setTempValue = text => this.label12.Text = text;
            Action<string> setLightValue = text => this.label15.Text = text;

            Action<Bitmap> setImg = bi=>this.pictureBox1.Image = bi;
            Action<String> remoteSend = text => this.streamView.richTextBox3.Text = text;

            if (this.streamView.richTextBox1.InvokeRequired)
            {

                if (mq_inited && mq_isopen && mqttClient.IsConnected)
                {
                    mqttClient.Publish("/tandl", Encoding.UTF8.GetBytes(value), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, false);
                    this.streamView.richTextBox3.Invoke(remoteSend, value);
                }

                try
                {
                    DataEntity jo = JsonConvert.DeserializeObject<DataEntity>(value);
                    if (!is_graph_stop) {
                        if (line_l_data.Count < this.buffer)
                        {
                            line_l_data.Add(jo.L);
                        }
                        else
                        {
                            for (int i = 0; i < line_l_data.Count - 1; i++)
                            {
                                line_l_data[i] = line_l_data[i + 1];
                            }
                            line_l_data[line_l_data.Count - 1] = jo.L;
                        }

                        if (line_t_data.Count < this.buffer)
                        {
                            line_t_data.Add(jo.T);
                        }
                        else
                        {
                            for (int i = 0; i < line_t_data.Count - 1; i++)
                            {
                                line_t_data[i] = line_t_data[i + 1];
                            }
                            line_t_data[line_t_data.Count - 1] = jo.T;
                        }
                    }
                    Bitmap bii = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
                    Graphics gg = Graphics.FromImage(bii);
                    //Random r = new Random();
                    string tt = "";
                    for(int i = 0;i<this.pictureBox1.Width;i+=this.grid){
                        for (int j = 0; j < this.pictureBox1.Height;j+=this.grid )
                        {
                            gg.DrawLine(new Pen(Color.Gray, 1),20,j,this.pictureBox1.Width-30,j);
                            if (i >= 20 && i <= this.pictureBox1.Width - 30)
                            {
                                gg.DrawLine(new Pen(Color.Gray, 1), i, 0, i, this.pictureBox1.Height);
                            }

                        }
                    }
                    for (int j = 0; j < line_t_data.Count-1;j++)
                    {
                        tt += Convert.ToSingle(line_t_data[j]) + ":" + Convert.ToSingle(line_l_data[j])+"-";
                        gg.DrawLine(new Pen(Color.Green, 1), (Single)j*500 / buffer, this.pictureBox1.Height/2 - Convert.ToSingle(line_t_data[j]) * 2, (Single)(j + 1)*500/buffer, this.pictureBox1.Height/2 - Convert.ToSingle(line_t_data[j + 1]) * 2);

                        if (j == line_t_data.Count - 2) {
                            gg.DrawEllipse(new Pen(Color.White,4),(Single)(j + 1)*500/buffer, this.pictureBox1.Height/2 - Convert.ToSingle(line_t_data[j + 1]) * 2,1,1);
                            gg.DrawEllipse(new Pen(Color.White, 4), (Single)(j + 1) * 500 / buffer, this.pictureBox1.Height/2 - Convert.ToSingle(line_l_data[j + 1]) / 10, 1, 1); 
      
                        }
                        gg.DrawLine(new Pen(Color.Red, 1), (Single)j*500/ buffer , this.pictureBox1.Height/2 - Convert.ToSingle(line_l_data[j]) / 10, (Single)(j + 1)*500/buffer, this.pictureBox1.Height/2 - Convert.ToSingle(line_l_data[j + 1]) / 10);
                   
                    }


                    gg.DrawLine(new Pen(Color.Blue,1),20,this.pictureBox1.Height/2,this.pictureBox1.Width-30,this.pictureBox1.Height/2);
                    
                    gg.DrawLine(new Pen(Color.Green, 1), 20,0, 20, this.pictureBox1.Height);
                    gg.DrawString("温度(t)",new Font("宋体",12),new SolidBrush(Color.Green),new PointF(0,0));
                    for (Single i =0;i<= this.pictureBox1.Height / 2; i += 20) 
                    {
                        gg.DrawString(Convert.ToString(i/2), new Font("宋体", 8), new SolidBrush(Color.Green), new PointF(0, this.pictureBox1.Height / 2 - i));
                        if (i / 2 != 0)
                        {
                            gg.DrawString("-" + Convert.ToString(i / 2), new Font("宋体", 8), new SolidBrush(Color.Green), new PointF(0, this.pictureBox1.Height / 2 + i));
                            gg.DrawString("-" + Convert.ToString(i * 10), new Font("宋体", 8), new SolidBrush(Color.Red), new PointF(this.pictureBox1.Width - 30, this.pictureBox1.Height / 2 + i));
                   
                        }
                        gg.DrawString(Convert.ToString(i*10), new Font("宋体", 8), new SolidBrush(Color.Red), new PointF(this.pictureBox1.Width-30, this.pictureBox1.Height / 2 - i));
             
                    }
                    gg.DrawLine(new Pen(Color.Red, 1),this.pictureBox1.Width-30,0,this.pictureBox1.Width-30, this.pictureBox1.Height);
                    gg.DrawString("光强(l)", new Font("宋体", 12), new SolidBrush(Color.Red), new PointF(this.pictureBox1.Width - 40, 0));
                    this.pictureBox1.Invoke(setImg, bii);
                    this.label12.Invoke(setTempValue, jo.T + "度");
                    this.label15.Invoke(setLightValue, jo.L + "");
                  
                    this.streamView.richTextBox1.Invoke(setDataReceived, value);
                }
                catch (Exception e)
                {
                    //showMessagge(false, e.Message);
                    //MessageBox.Show(e.ToString());
                    this.streamView.richTextBox1.Invoke(setDataReceived,e.ToString());
                }
                finally
                {
                    Listening = false;//我用完了，ui可以关闭串口了。  
                }

            }
            else
            {
                setDataReceived(value);
            }
        }
        public string totwo(string a) {
            byte[] data = Encoding.Unicode.GetBytes(a);
            StringBuilder sb = new StringBuilder(data.Length * 8);

            foreach (byte b in data) {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }
      
        private string ReadSerialData()
        {
            string value = "";
            try
            {
                this.Listening = true;
                if (port != null && port.BytesToRead > 0)
                {
                    value = port.ReadLine();
                    
                }
            }
            catch (Exception ex)
            {
                //label3.Text = "读取串口数据发生错误：" + ex.Message;
                showMessagge(false,ex.Message);
            }

            return value;
        }

        private void ChangeArduinoSendStatus(bool v)
        {
            if (port != null && port.IsOpen)
            {
                if (v)
                {
                    port.WriteLine("serial start");
                }
                else
                {
                    port.WriteLine("serial stop");
                }
            }
        }

        private void DisposeSerialPort()
        {
            if (port!=null&&port.IsOpen)
            {
                Closing = true;
                while (Listening) Application.DoEvents();
                //打开时点击，则关闭串口  
                port.Close();
                Closing = false;
            }
            if (port != null)
            {
                try
                {
                    this.ChangeArduinoSendStatus(false);
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                    port.Dispose();
                    this.label13.Text = "已关闭";
                    this.label13.ForeColor = Color.Red;
                    this.pictureBox2.Image = this.port_off_pic.Image;
                }
                catch (Exception ex)
                {
                    showMessagge(false, "关闭串口发生错误：" + ex.Message);
                    //this.label3.Text = "关闭串口发生错误：" + ex.Message;
                    this.label13.ForeColor = Color.Red;

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.port_start_pic.Visible = false;
            this.port_off_pic.Visible = false;
            this.send_off.Visible = false;
            this.send_on.Visible = false;
            this.comboBox1.Items.AddRange(SerialPort.GetPortNames());
            this.comboBox1.SelectedIndex = this.comboBox1.Items.Count - 1;//Arduino一般在最后一个串口
            bitmap = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            g = Graphics.FromImage(bitmap);
            pictureBox1.Image = bitmap;
            comboBox1.BackColor = Color.FromArgb(255, 44, 44, 44);
            textBox4.BackColor = Color.FromArgb(255, 44, 44, 44);
            textBox5.BackColor = Color.FromArgb(255, 44, 44, 44);
            textBox7.BackColor = Color.FromArgb(255, 44, 44, 44);

            pictureBox6.BackColor = Color.FromArgb(255, 44, 44, 44);
            messagebb.Visible = false;
            messagebb.Height = 0;

            debug = new Debug(new SetVisiableHandler(changeDebugView), new DataSendHandler(changeRGB));
            streamView = new StreamView(new SetVisiableHandler(changeStreamView));
            aboutView = new About(new SetVisiableHandler(changeAboutView));
            string[] bote = {"300", "600", "1200", "2400", "4800", "9600", "19200", "38400", "43000", "56000", "57600", "115200"};
            comboBox2.Items.AddRange(bote);
            comboBox2.BackColor = Color.FromArgb(255, 44, 44, 44);
            this.comboBox2.SelectedIndex = this.comboBox2.Items.Count - 7;

            
        }

        public void showMessagge(bool status,string message) {
            Action<bool> setMessageVis = text => messagebb.Visible = text;
            Action<int> setMessageHeight = text => messagebb.Height = text;
            Action<string> setMessageText = text => messagemm.Text = text;
            Action<Color> setMessageTColor = text => messagemm.ForeColor = text;

            Action<string> setMessageTText = text => messagett.Text = text;
            Action<Image> setMessageTImage = text => messagess.Image = text;

            messagebb.Invoke(setMessageVis, true);
            for (int i = 0;i<180;i+=20) {
                messagebb.Invoke(setMessageHeight,i);
            }
            messagemm.Invoke(setMessageText, message);
            if (status)
            {
                messagett.Invoke(setMessageTText, "提示：");
                messagett.Invoke(setMessageTColor, Color.Green);
                messagess.Invoke(setMessageTImage,global::WindowsFormsApp1.Properties.Resources.messageinfo_s);
            }
            else {
                messagett.Invoke(setMessageTText, "提示：");
                messagett.Invoke(setMessageTColor, Color.Red);
                messagess.Invoke(setMessageTImage, global::WindowsFormsApp1.Properties.Resources.messageinfo_f);
            }
        }
        public void closeMessage() {
            for (int i = 180; i > 0; i -= 10)
            {
                messagebb.Height = i;
            }
            messagebb.Visible = false;
        }
       // private void textBox2_TextChanged(object sender, EventArgs e)
       // {
       //     this.textBox2.SelectionStart = this.textBox2.Text.Length;
       //     this.textBox2.SelectionLength = 0;
       //     this.textBox2.ScrollToCaret();
       // }

      

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        

     

      

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

      

        private void label17_Click(object sender, EventArgs e)
        {
            this.InitialSerialPort();
        }

        private void label18_Click(object sender, EventArgs e)
        {
            this.DisposeSerialPort();
        }

        private void label19_Click(object sender, EventArgs e)
        {
          
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string ss = textBox4.Text == "" ? "100" : textBox4.Text;
            this.buffer = int.Parse(ss);
            this.line_l_data.Clear();
            this.line_t_data.Clear();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            string ss = textBox5.Text == "" ? "5" : textBox5.Text;
            this.grid = int.Parse(ss);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
       
      

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = "C:\\";
            open.Filter = "SmartControlFile|*.smtc;*.smt;*.json;*.xml";
            open.RestoreDirectory = true;
            open.FilterIndex = 1;
            if(open.ShowDialog()==DialogResult.OK){
                
                try
                {
                    switch (open.FileName.Split('.')[1]) {
                        case "smtc":
                            SaveControlDto control = utils.EntityGen<SaveControlDto>.ReadFile(open.FileName);
                            
                            loadControlFile(control);
                            break;
                        default:
                            SaveDataDto data = utils.EntityGen<SaveDataDto>.ReadFile(open.FileName);
                            loadData(data);
                            break;
                    }
                   
                }
                catch (IOException eee){
                    showMessagge(false,"文件读取异常！");
                }
            }
        }

        private void loadData(SaveDataDto data)
        {

            if (data.Version.Equals(VERSION))
            {
                this.grid = int.Parse(data.Grid_size);
                this.textBox5.Text = data.Grid_size;
                this.buffer = int.Parse(data.Buffer_size);
                this.textBox4.Text = data.Buffer_size;
                this.line_l_data.Clear();
                this.line_t_data.Clear();

                this.line_t_data.Clear();
                this.line_l_data = data.Data_l;
                this.line_t_data.Clear();
                this.line_t_data = data.Data_t;

            }
            else
            {
                showMessagge(false, "文件版本[" + data.Version + "]与当前软件版本[" + VERSION + "]不一致！");
            }

           
              
        }

        private void loadControlFile(SaveControlDto control)
        {


            if (control.Version.Equals(VERSION)) { 
                if (control.Light_status.Equals("1"))
                {
                    ll_con.Image = global::WindowsFormsApp1.Properties.Resources.light_of;
                    power_con = "1";
                }
                else
                {
                    ll_con.Image = global::WindowsFormsApp1.Properties.Resources.light_ooof;
                    power_con = "-1";
                }
                
                
                track_index_r.Left = Convert.ToInt32(Math.Ceiling(int.Parse(control.R) / 2.55));
                tr_r = Convert.ToByte(int.Parse(control.R));
                track_index_r.BackColor = Color.FromArgb(255, tr_r, 0, 0);
                pictureBox7.BackColor = Color.FromArgb(255, tr_r, tr_g, tr_b);
                
                track_index_g.Left = Convert.ToInt32(Math.Ceiling(int.Parse(control.G) / 2.55));
                tr_g = Convert.ToByte(int.Parse(control.G));
                track_index_g.BackColor = Color.FromArgb(255, 0, tr_g, 0);
                pictureBox7.BackColor = Color.FromArgb(255, tr_r, tr_g, tr_b);
                
                track_index_b.Left = Convert.ToInt32(Math.Ceiling(int.Parse(control.B) / 2.55));
                tr_b = Convert.ToByte(int.Parse(control.B));
                track_index_b.BackColor = Color.FromArgb(255, 0, 0, tr_b);
                pictureBox7.BackColor = Color.FromArgb(255, tr_r, tr_g, tr_b);
                            
            }
            else
            {
                showMessagge(false, "文件版本[" + control.Version + "]与当前软件版本[" + VERSION + "]不一致！");
            }
                   
           
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = "C://";
            save.Filter = "SmartControlFile(*.smtc)|*.smtc";
            if(save.ShowDialog()==DialogResult.OK){
                FileGen<SaveControlDto>.FileWrite(FileGen<SaveControlDto>.dataFileGen(FileType.control,new SaveControlDto(VERSION, power_con, tr_r.ToString(), tr_g.ToString(), tr_b.ToString())), save.FileName);
            }
        }

        private void 保存缓冲区数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = "C://";
            save.Filter = "SmartControlData(*.smt)|*.smt";
            if (save.ShowDialog() == DialogResult.OK)
            {
                FileGen<SaveDataDto>.FileWrite(FileGen<SaveDataDto>.dataFileGen(FileType.smt, new SaveDataDto("smt", VERSION, Convert.ToString(buffer), Convert.ToString(grid), line_t_data, line_l_data)),save.FileName);
            }
        }

      
        private void SaveBufferDataToJson_click(object sender, EventArgs e){
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = "C://";
            save.Filter = "SmartControlData(*.json)|*.json";
            if (save.ShowDialog() == DialogResult.OK)
            {

                FileGen<SaveDataDto>.FileWrite(FileGen<SaveDataDto>.dataFileGen(FileType.json, new SaveDataDto("json", VERSION, Convert.ToString(buffer), Convert.ToString(grid), line_t_data, line_l_data)),save.FileName);
            }
        }
        private void SaveBufferDataToXml_click(object sender, EventArgs e){
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = "C://";
            save.Filter = "SmartControlData(*.xml)|*.xml";
            if (save.ShowDialog() == DialogResult.OK)
            {
                FileGen<SaveDataDto>.FileWrite(FileGen<SaveDataDto>.dataFileGen(FileType.xml, new SaveDataDto("xml", VERSION, Convert.ToString(buffer), Convert.ToString(grid), line_t_data, line_l_data)),save.FileName);
            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            aboutView.StartPosition = FormStartPosition.Manual;
            aboutView.Location = new Point(this.Location.X + 30, this.Location.Y+30);
            aboutView.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }


        bool is_sss = false;
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (!is_sss)
            {
                this.InitialSerialPort();
                this.pictureBox2.Image = global::WindowsFormsApp1.Properties.Resources.off;
                this.pictureBox1.BackgroundImage = null;
                is_sss = true;
            }
            else {
                this.DisposeSerialPort();
                this.pictureBox2.Image = global::WindowsFormsApp1.Properties.Resources.close;
                is_sss = false;
            }
        }
        private void pictureBox3_mousein(object sender, EventArgs e) {
            pictureBox3.Image = global::WindowsFormsApp1.Properties.Resources.send2;
        }
        private void pictureBox3_leave(object sender, EventArgs e)
        {
            pictureBox3.Image = global::WindowsFormsApp1.Properties.Resources.sned1;
        }

        private void exit_P_o(object sender, EventArgs e) {
            pictureBox6.Image = global::WindowsFormsApp1.Properties.Resources.ggb;
        }
        private void exit_P_f(object sender, EventArgs e)
        {
            pictureBox6.Image = global::WindowsFormsApp1.Properties.Resources.gg;
        }

        string power_con = "-1";
        private void pictureBox3_Click(object sender, EventArgs e)
        {
           
             
            string controlColor = "{\"power\":\"" + power_con + "\",\"r\":\"" + tr_r.ToString() + "\",\"g\":\"" + tr_g.ToString() + "\",\"b\":\"" + tr_b.ToString() + "\"}";

            this.changeRGB(controlColor);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void ll_con_Click(object sender, EventArgs e)
        {
            if (power_con.Equals("1"))
            {
                power_con = "-1";
                ll_con.Image = global::WindowsFormsApp1.Properties.Resources.light_ooof;
            }
            else {
                power_con = "1";
                ll_con.Image = global::WindowsFormsApp1.Properties.Resources.light_of;
            }
          
        }
      
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void messagecc_Click(object sender, EventArgs e)
        {
            closeMessage();
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void track_r_Click(object sender, EventArgs e)
        {

        }

     
        private void changeDebugView() {
            if (!is_stream_open)
            {
                is_debug_open = false;
            }
            else {
                is_debug_open = false;
                stream_open();
                stream_open();
               
            }
           
        }
        private void changeStreamView()
        {
            if (!is_debug_open)
            {
                is_stream_open = false;
            }
            else {
                is_stream_open = false;
                debug_open();
                debug_open();
               
            }
        }
        private void changeAboutView()
        {
            is_about_open = false;
        }
        bool is_debug_open = false;
        bool is_stream_open = false;
        bool is_about_open = false;
      
        private void debug_open() {
            if (!is_debug_open)
            {
                debug.StartPosition = FormStartPosition.Manual;
                debug.Location = new Point(this.Location.X, this.Location.Y + this.Height);

                debug.Height = 60;

                if (is_stream_open)
                {
                    debug.Width = this.Width + streamView.Width;
                    debug.richTextBox1.Width = Convert.ToInt32(Math.Ceiling(debug.Width * 0.77));
                }
                else
                {
                    debug.Width = this.Width;
                    debug.richTextBox1.Width = Convert.ToInt32(Math.Ceiling(debug.Width * 0.74));
                }


                debug.pictureBox1.Left = debug.Width - debug.pictureBox1.Width - 10;
                debug.pictureBox3.Left = debug.richTextBox1.Left + debug.richTextBox1.Width + 20;
                debug.Show();
                is_debug_open = true;
            }
            else
            {
                debug.Hide();
                if (is_stream_open) {
                    streamView.Height = this.Height;
                }
                is_debug_open = false;
            }
        }
        private void stream_open() {
            if (!is_stream_open)
            {
                streamView.StartPosition = FormStartPosition.Manual;
                streamView.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                if (is_debug_open)
                {
                    streamView.Height = this.Height + debug.Height;
                }
                else
                {
                    streamView.Height = this.Height;
                }

                streamView.Width = 200;

                streamView.Show();
                is_stream_open = true;
            }
            else
            {
                streamView.Hide();
                if (is_debug_open)
                {
                    debug.Width = this.Width;
                    debug.richTextBox1.Width = Convert.ToInt32(Math.Ceiling(debug.Width * 0.74));
                    debug.pictureBox1.Left = debug.Width - debug.pictureBox1.Width - 10;
                    debug.pictureBox3.Left = debug.richTextBox1.Left + debug.richTextBox1.Width + 20;
                }
                is_stream_open = false;
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debug_open();
        }
        private void 数据接收器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stream_open();
        }

        private void 视图VToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (is_debug_open)
            {
                debugToolStripMenuItem.Image = global::WindowsFormsApp1.Properties.Resources.view_is_open;
            }
            else {
                debugToolStripMenuItem.Image = global::WindowsFormsApp1.Properties.Resources.view_default;
            }
            if (is_stream_open)
            {
                数据接收器ToolStripMenuItem.Image = global::WindowsFormsApp1.Properties.Resources.view_is_open;
            }
            else
            {
                数据接收器ToolStripMenuItem.Image = global::WindowsFormsApp1.Properties.Resources.view_default;
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = "SmartControl_"+buffer+"_"+grid+"_img";
            save.Filter = "(.jpg)|*.jpg";

            if (save.ShowDialog() == DialogResult.OK)
            {
                Bitmap bit = new Bitmap(pictureBox1.ClientRectangle.Width, pictureBox1.ClientRectangle.Height);
                pictureBox1.DrawToBitmap(bit, pictureBox1.ClientRectangle);
                bit.Save(save.FileName);
            }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            buffer = int.Parse(textBox7.Text)*60;
            textBox4.Text = Convert.ToString(buffer);

        }

        private void 远程连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        Boolean mq_inited = false;
        Boolean mq_isopen = false;
        String mqClientId = Guid.NewGuid().ToString();
        private void 启动远程控制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!mq_inited) {
                mqttClient = new MqttClient(BROKER_IP);
                mq_inited = true;
                mqttClient.MqttMsgPublishReceived += ClientMqttReceived;//绑定消息接收
                mqttClient.ConnectionClosed += ClientMqttLost;//断线
            }

            if (!mq_isopen)
            {
                try
                {
                    mqttClient.Connect(mqClientId);
                    mqttClient.Subscribe(new string[] { "/control" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });//订阅远程控制主题
                    启动远程控制ToolStripMenuItem.Text = "关闭远程控制";
                    streamView.remote_data.ForeColor = Color.Green;
                    streamView.remote_data.Text = "远程连接成功";
                    mq_isopen = true;
                }
                catch (Exception eeee) {
                    showMessagge(false, eeee.Message);
                    streamView.remote_data.ForeColor = Color.Red;
                    streamView.remote_data.Text = eeee.ToString();
                }

            }else {
                if (mqttClient.IsConnected)
                {
                    mqttClient.Disconnect();
                    mq_isopen = false;
                    启动远程控制ToolStripMenuItem.Text = "打开远程控制";
                }
            }
        }

        private void ClientMqttLost(object sender, EventArgs e)
        {
            Action<string> setDataReceived = text => this.streamView.remote_data.Text = text;
            if (this.streamView.remote_data.InvokeRequired)
            {
                streamView.remote_data.Invoke(setDataReceived, "远程连接断开");
            }
           
          
        }


        public void remoteRun() {
            this.pictureBox3_Click(null,null);
        }
        private void ClientMqttReceived(object sender, MqttMsgPublishEventArgs e)
        {
            String msg = System.Text.Encoding.UTF8.GetString(e.Message);

            Action<string> setDataReceived = text => this.streamView.remote_data.Text = text;
            Action<int> setRLeft = text => this.track_index_r.Left = text+10;
            Action<int> setGLeft = text => this.track_index_g.Left = text+10;
            Action<int> setBLeft = text => this.track_index_b.Left = text+10;

            Action<Color> setRColor = text => this.track_index_r.BackColor = text;
            Action<Color> setGColor = text => this.track_index_g.BackColor = text;
            Action<Color> setBColor = text => this.track_index_b.BackColor = text;

            Action<Image> setCon = text => this.ll_con.Image = text;

            Action<Color> setLColor = text => this.pictureBox7.BackColor = text;
            if (this.streamView.remote_data.InvokeRequired)
            {
                try
                {
                    streamView.remote_data.Invoke(setDataReceived, msg);
                    DataEntity jo = JsonConvert.DeserializeObject<DataEntity>(msg);
                    track_index_r.Invoke(setRLeft, Convert.ToInt32(Math.Ceiling(int.Parse(jo.R) / 2.55)));
                    track_index_r.Invoke(setRColor, Color.FromArgb(255, int.Parse(jo.R), 0, 0));
                    tr_r = Convert.ToByte(int.Parse(jo.R));
                    track_index_g.Invoke(setGLeft, Convert.ToInt32(Math.Ceiling(int.Parse(jo.G) / 2.55)));
                    track_index_g.Invoke(setGColor, Color.FromArgb(255, 0, int.Parse(jo.G), 0));
                    tr_g = Convert.ToByte(int.Parse(jo.G));
                    track_index_b.Invoke(setBLeft, Convert.ToInt32(Math.Ceiling(int.Parse(jo.B) / 2.55)));
                    track_index_b.Invoke(setBColor, Color.FromArgb(255, 0, 0, int.Parse(jo.B)));
                    tr_b = Convert.ToByte(int.Parse(jo.B));

                    if (jo.Power.Equals("1"))
                    {
                        ll_con.Invoke(setCon, global::WindowsFormsApp1.Properties.Resources.light_of);
                        power_con = "1";
                    }
                    else
                    {
                        ll_con.Invoke(setCon, global::WindowsFormsApp1.Properties.Resources.light_ooof);
                        power_con = "-1";
                    }

                    pictureBox7.Invoke(setLColor, Color.FromArgb(255, int.Parse(jo.R), int.Parse(jo.G), int.Parse(jo.B)));
                    RemoteRun click = new RemoteRun(remoteRun);
                    Invoke(click);
                }
                catch (Exception eeee) {
                    this.streamView.remote_data.Invoke(setDataReceived, msg);
                }
              
            }
        }
        private void g_move(object sender, MouseEventArgs e)
        {
            //showMessagge(true,"x:"+e.X+" y:"+e.Y);
        }

        bool is_graph_stop = false;
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            if (is_graph_stop)
            {
                pictureBox8.Image = global::WindowsFormsApp1.Properties.Resources.stop;
                is_graph_stop = !is_graph_stop;
            }
            else {
                pictureBox8.Image = global::WindowsFormsApp1.Properties.Resources.start;
                is_graph_stop = !is_graph_stop;
            }

        }
    }
}


