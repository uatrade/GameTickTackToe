using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameTickTackToeUDP
{

    public partial class MainWindow : Window
    {
        string BtnClickName;
        string RadioUser1, RadioUser2;
        int portSend, portReceive;

        List<Button> btnlist=new List<Button>();
        public MainWindow()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(Colors.Aqua);
            Row0Col0.Click += anyButton_Click;
            Row1Col0.Click += anyButton_Click;
            Row2Col0.Click += anyButton_Click;
            Row0Col1.Click += anyButton_Click;
            Row1Col1.Click += anyButton_Click;
            Row2Col1.Click += anyButton_Click;
            Row0Col2.Click += anyButton_Click;
            Row1Col2.Click += anyButton_Click;
            Row2Col2.Click+= anyButton_Click;

            btnlist.Add(Row0Col0);
            btnlist.Add(Row1Col0);
            btnlist.Add(Row2Col0);
            btnlist.Add(Row0Col1);
            btnlist.Add(Row1Col1);
            btnlist.Add(Row2Col1);
            btnlist.Add(Row0Col2);
            btnlist.Add(Row1Col2);
            btnlist.Add(Row2Col2);
        }

        void anyButton_Click(object sender, EventArgs e) 
        {
            BtnClickName = (sender as Button).Name;

            foreach (var item in btnlist)
            {
                if (item.Name == BtnClickName)
                {
                   // item.Content = RadioUser1;

                    item.Dispatcher.Invoke(new Action(() =>
                    {
                       item.Content = RadioUser1;
                        item.IsEnabled = false;
                    }));

                    SendInfo();
                }
            }
            CheckWin();

        }
        private void CheckRadioBtn()
        {
            if(rbtn0.IsChecked==true)
            {
                portSend = 9006;
                portReceive = 9005;
                RadioUser1 = "0";
                RadioUser2 = "X";
            }
            if (rbtnX.IsChecked == true)
            {
                portSend = 9005;
                portReceive = 9006;
                RadioUser1 = "X";
                RadioUser2 = "0";
            }
        }

        private void SendInfo()
        {

            UdpClient user1 = new UdpClient();

                try
                {

                if (BtnClickName != "")
                {
                    string message = BtnClickName;

                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    user1.Send(bytes, bytes.Length, "127.0.0.1", portSend);
                }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    user1.Close();
                }
        }

        private void ReceiveInfo()
        {
            //MessageBox.Show(portReceive.ToString());
            UdpClient user2 = new UdpClient(portReceive);
            IPEndPoint ip = null;
            try
                {

                while (true)
                {
                    byte[] bytes = user2.Receive(ref ip);
                    string message = Encoding.UTF8.GetString(bytes);

                    if(message=="%&!WIN")  //код победы
                    {

                        OpponCounWin.Dispatcher.Invoke(new Action(()=>
                        {
                            OpponCounWin.Text = (Int32.Parse(OpponCounWin.Text) + 1).ToString();
                        }
                            ));

                        //MessageBox.Show($"Победил {Opponent.Text}");
                    }

                    if(message[0]=='&')    //Получение имени
                        {
                        Opponent.Dispatcher.Invoke(new Action(()=>
                            {
                                Opponent.Text = message.Replace("&", "");
                            }
                            ));
                        }
                    else
                    {
                        for (int i = 0; i < btnlist.Count; i++)
                        {
                            //btnlist[i].Content = RadioUser2;
                            btnlist[i].Dispatcher.Invoke(new Action(() =>
                            {
                                //MessageBox.Show(btnlist[i].Name+"="+message);
                                if (btnlist[i].Name == message)
                                {
                                    //MessageBox.Show(btnlist[i].Name + "=" + message);
                                    btnlist[i].Content = RadioUser2;
                                    btnlist[i].IsEnabled = false;
                                }
                            }));

                        }
                    }

                }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            finally
            {
                user2.Close();
            }
            //CheckWin();
        }

        private void SendName()
        {
            UdpClient client = new UdpClient();
            try
            {
                while (true)
                {
                    YourName.Dispatcher.Invoke(new Action(()=>
                        {
                        byte[] bytes = Encoding.UTF8.GetBytes("&" + YourName.Text);
                        client.Send(bytes, bytes.Length, "127.0.0.1", portSend);
                        }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }

        private void WinName()
        {
            UdpClient client = new UdpClient();
            try
            {
                    YourName.Dispatcher.Invoke(new Action(() =>
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes("%&!WIN");  //отправка кода победы
                        client.Send(bytes, bytes.Length, "127.0.0.1", portSend);
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }


        private void CheckWin()
        {
            try
            {
                if (Row0Col0.Content == Row1Col0.Content && Row1Col0.Content == Row2Col0.Content && Row0Col0.IsEnabled==false || 
                    Row0Col0.Content == Row0Col1.Content && Row0Col0.Content == Row0Col2.Content && Row0Col0.IsEnabled==false ||
                    Row0Col0.Content == Row1Col1.Content && Row0Col0.Content == Row2Col2.Content && Row0Col0.IsEnabled==false ||
                    Row1Col0.Content == Row1Col1.Content && Row1Col0.Content == Row1Col2.Content && Row1Col0.IsEnabled==false ||
                    Row2Col0.Content == Row2Col1.Content && Row2Col0.Content == Row2Col2.Content && Row2Col0.IsEnabled==false || 
                    Row0Col1.Content == Row1Col1.Content && Row0Col1.Content == Row2Col1.Content && Row0Col1.IsEnabled==false || 
                    Row0Col2.Content == Row1Col2.Content && Row0Col2.Content == Row2Col2.Content && Row0Col2.IsEnabled==false || 
                    Row0Col2.Content == Row1Col1.Content && Row0Col2.Content == Row2Col0.Content && Row0Col2.IsEnabled==false)
                {
                    if(Row0Col0.Content.ToString()=="X"|| Row1Col0.Content.ToString()=="X"|| Row2Col0.Content.ToString()=="X"|| Row0Col1.Content.ToString()=="X"
                        || Row0Col2.Content.ToString()=="X")
  
                        if (rbtnX.IsChecked == true)
                        {
                            UserWinCount.Text = (Int32.Parse(UserWinCount.Text) + 1).ToString();
                            Task taskName = new Task(WinName);
                            taskName.Start();
                            MessageBox.Show($"Победил {UserName.Text}");
                        }


                    if (Row0Col0.Content.ToString() == "0" || Row1Col0.Content.ToString() == "0" || Row2Col0.Content.ToString() == "0" || Row0Col1.Content.ToString() == "0"
                        || Row0Col2.Content.ToString() == "0")
                    {
                        if (rbtn0.IsChecked == true)
                        {
                            UserWinCount.Text = (Int32.Parse(UserWinCount.Text) + 1).ToString();
                            Task taskName = new Task(WinName);
                            taskName.Start();
                            MessageBox.Show($"Победил {UserName.Text}");
                        }
                    }

                    for(int i=0;i<btnlist.Count;i++)
                        btnlist[i].IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            CheckRadioBtn();
            Task taskReceive = new Task(ReceiveInfo);

            taskReceive.Start();

            Task taskName = new Task(SendName);
            taskName.Start();
            btnStart.IsEnabled = false;
            YourName.IsEnabled = false;
            rbtn0.IsEnabled = false;
            rbtnX.IsEnabled = false;
        }
        private void btnElseOne_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < btnlist.Count; i++)
            {
                btnlist[i].IsEnabled = true;
                btnlist[i].Content = " ";
            }
        }
    }
}
