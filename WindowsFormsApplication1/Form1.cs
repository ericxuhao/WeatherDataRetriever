using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TestCookie sc = new TestCookie();
        //ArrayList sProvinces = new ArrayList();
        private static string selectedProv = "";
        public string SelectedProv
        {
            get { return selectedProv; }
            set { selectedProv = value; }
        }
        public class TestCookie
        {
            public CookieContainer mycookie = new CookieContainer();//定义cookie容器
            public Object obj = new Object();
            public byte[] byt = new byte[1];
            public void upcookie(CookieCollection cookie)
            {
                for (int i = 0; i < cookie.Count; i++)
                {
                    mycookie.Add(cookie[i]);
                }
                obj = mycookie;
                byt = ObjectToBytes(obj);
            }
            /**/
            /// <summary>
            /// 将一个object对象序列化，返回一个byte[]
            /// </summary>
            /// <param name="obj">能序列化的对象</param>
            /// <returns></returns>
            public static byte[] ObjectToBytes(object obj)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, obj);
                    return ms.GetBuffer();
                }
            }

            /**/
            /// <summary>
            /// 将一个序列化后的byte[]数组还原
            /// </summary>
            /// <param name="Bytes"></param>
            /// <returns></returns>
            public object BytesToObject(byte[] Bytes)
            {
                using (MemoryStream ms = new MemoryStream(Bytes))
                {
                    IFormatter formatter = new BinaryFormatter();
                    return formatter.Deserialize(ms);
                }
            }
            public CookieContainer getcookie()
            {
                return mycookie;
            }
        }
        public static Image doGetImg(string Url, TestCookie bCookie)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url.ToString());
                myRequest.ServicePoint.Expect100Continue = true;
                myRequest.CookieContainer = bCookie.mycookie;
                myRequest.Method = "GET";
                myRequest.Timeout = 30000;
                myRequest.KeepAlive = true;//modify by yang
                myRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0)";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                bCookie.upcookie(myResponse.Cookies);
                return Bitmap.FromStream(myResponse.GetResponseStream());
            }
            catch
            {
                return null;
            }
        }
        public static string doGet(string Url, TestCookie bCookie, String encodingFormat, String referer)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url.ToString());
                Stream getStream;
                StreamReader streamReader;
                myRequest.ServicePoint.Expect100Continue = true;
                myRequest.CookieContainer = bCookie.mycookie;
                myRequest.Method = "GET";
                myRequest.Accept = "*/*";
                myRequest.Referer = referer;
                myRequest.Timeout = 30000;
                myRequest.KeepAlive = true;//modify by yang
                myRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0)";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                bCookie.upcookie(myResponse.Cookies);
                getStream = myResponse.GetResponseStream();
                streamReader = new StreamReader(getStream, Encoding.GetEncoding(encodingFormat));
                string getString = streamReader.ReadToEnd();
                return getString;
            }
            catch
            {
                return null;
            }
        }
        public static string doPost(string Url, byte[] postData, TestCookie bCookie, String encodingFormat, String referer)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url.ToString());
                myRequest.CookieContainer = bCookie.mycookie;
                myRequest.Method = "POST";
                myRequest.Timeout = 30000;
                myRequest.KeepAlive = true;
                if (referer != "")
                    myRequest.Referer = referer;
                myRequest.Headers["Cache-control"] = "no-store, no-cache, must-revalidate, post-check=0, pre-check=0";
                myRequest.Headers["Accept-Language"] = "zh-CN,zh;q=0.8,en-US;q=0.5,en;q=0.3";
                myRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0)";
                myRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                myRequest.Accept = "*/*";
                myRequest.ContentLength = postData.Length;
                Stream newStream = myRequest.GetRequestStream();
                newStream.Write(postData, 0, postData.Length);
                newStream.Close();
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                bCookie.upcookie(myResponse.Cookies);
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(encodingFormat));
                string outdata = reader.ReadToEnd();
                reader.Close();
                if (!outdata.Contains("基础连接已经关闭: 连接被意外关闭") && !outdata.Contains("无法连接到远程服务器") && !outdata.Contains("基础连接已经关闭: 接收时发生错误。"))
                    return outdata;
                else
                    return "基础连接已经关闭: 连接被意外关闭";
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("基础连接已经关闭: 连接被意外关闭") && !ex.Message.Contains("无法连接到远程服务器") && !ex.Message.Contains("基础连接已经关闭: 接收时发生错误。"))
                    return ex.Message;
                else
                    return "基础连接已经关闭: 连接被意外关闭";
            }

        }

        private string getPostData(string provCode, TestCookie sc)
        {
            string poststr1 = "act=getStationsByProvinceID&provinceID=" + provCode + "&dataCode=A.0012.0001";
            byte[] data1 = System.Text.Encoding.UTF8.GetBytes(poststr1);
            string response1 = doPost("http://data.cma.cn/dataService/ajax.html", data1, sc, "utf-8", "http://data.cma.cn/dataService/index/datacode/A.0012.0001.html");
            string stationIDs = "";
            string elementIDs = "";
            JObject jo = (JObject)JsonConvert.DeserializeObject(response1);
            JToken records = jo["stations"];
            foreach (JToken record in records)
            {
                JProperty jp = (JProperty)record.First;
                string ID = jp.Value.ToString();
                stationIDs = stationIDs+ID + ",";
            }
            string dataCode = "A.0012.0001";
            if (provCode.Equals("710") || provCode.Equals("810") || provCode.Equals("820")) dataCode = "A.0013.0001";
            string poststr2 = "act=getElementsByDataCode&provinceID=" + provCode + "&dataCode=" + dataCode;
            byte[] data2 = System.Text.Encoding.UTF8.GetBytes(poststr2);
            string response2 = doPost("http://data.cma.cn/dataService/ajax.html", data2, sc, "utf-8", "http://data.cma.cn/dataService/index/datacode/A.0012.0001.html");
            JObject jo2 = (JObject)JsonConvert.DeserializeObject(response2);
            JToken elements = jo2["elements"];
            
            string regex = "id=\"selectone1\" value=\"(\\w+)\"";
            Regex re = new Regex(regex);
            MatchCollection matches = re.Matches(elements.ToString());
            System.Collections.IEnumerator enu = matches.GetEnumerator();
            while (enu.MoveNext() && enu.Current != null)
            {
                Match match = (Match)(enu.Current);
                string element = match.Value.Substring(23);
                elementIDs = elementIDs + element.Substring(0, element.Length - 1) + ",";                
            }
            stationIDs = stationIDs.Substring(0, stationIDs.Length - 1);
            elementIDs = elementIDs.Substring(0, elementIDs.Length - 1);
            //MessageBox.Show(stationIDs);
            //MessageBox.Show(elementIDs);
            StringBuilder postdata = new StringBuilder("");
            string[] s_stations = stationIDs.Split(',');
            string[] s_elements = elementIDs.Split(',');
            string dateE = this.dateTimePicker2.Value.ToString("yyyy-MM-dd 23");
            string dateS = this.dateTimePicker1.Value.ToString("yyyy-MM-dd 00");
            //MessageBox.Show(dateS+"---"+dateE);
            postdata.Append("dataCode=A.0012.0001&dataCodeInit=" + dataCode + "&dateE=" + dateE + "&dateS=" + dateS);
            postdata.Append("&hidden_limit_timeRange=7&hidden_limit_timeRangeUnit=Day&select=on&isRequiredHidden[]=dateS&isRequiredHidden[]=dataE&isRequiredHidden[]=station_ids[]&isRequiredHidden[]=elements[]&");
            foreach (string s_station in s_stations)
            {
                postdata.Append("station_ids[]="+s_station + "&");
            }
            foreach (string s_element in s_elements)
            {
                postdata.Append("elements[]="+s_element + "&");
            }
            string poststr = postdata.ToString();
            poststr = poststr.Substring(0,poststr.Length-1);
            //MessageBox.Show(poststr);
            return poststr;
        }

        
        private string submitOrder(string provCode, TestCookie sc)
        {
            string dataCode = "A.0012.0001";
            if (provCode.Equals("710") || provCode.Equals("810") || provCode.Equals("820")) dataCode = "A.0013.0001";
            string poststr2 = this.getPostData(provCode, sc);
            byte[] data2 = System.Text.Encoding.UTF8.GetBytes(poststr2);
            string response2 = doPost("http://data.cma.cn/data/search.html?dataCode=A.0012.0001", data2, sc, "utf-8", "http://data.cma.cn/dataService/index/datacode/A.0012.0001.html");
            string regex1 = "id=\"SearchCond\" value=\"(\\w+)\"";
            Regex re1 = new Regex(regex1);
            string regex2 = "id=\"SearchCondPage\" value=\"(\\w+)\"";
            Regex re2 = new Regex(regex2);
            MatchCollection matches1 = re1.Matches(response2);
            string SearchCond = matches1[0].ToString();
            SearchCond = SearchCond.Substring(23);
            MatchCollection matches2 = re2.Matches(response2);
            string SearchCondPage = matches2[0].ToString();
            SearchCondPage = SearchCondPage.Substring(27);
            SearchCond = SearchCond.Substring(0, SearchCond.Length - 1);
            SearchCondPage = SearchCondPage.Substring(0, SearchCondPage.Length - 1);
            //MessageBox.Show(SearchCond);
            //MessageBox.Show(SearchCondPage);
            string poststr3 = "SearchCond=" + SearchCond + "&&storageType=0&fileNum=0&SearchCondPage=" + SearchCondPage + "&code="+dataCode+"&selectFileInfo=";
            byte[] data3 = System.Text.Encoding.UTF8.GetBytes(poststr3);
            string response3 = doPost("http://data.cma.cn/order/car.html", data3, sc, "utf-8", "http://data.cma.cn/data/search.html?dataCode=A.0012.0001");
            return response3;
            //string url = "http://data.cma.cn/order/createOrder.html?" + carList;
        }

        private void resetCarview(TestCookie sc)
        {
            string html = doGet("http://data.cma.cn/order/carView.html", sc, "utf-8", "http://data.cma.cn/order/carView.html");
            string regex = "name=\"carlist\\[(\\w+)\\]\\[list\\]\"";
            Regex re = new Regex(regex);
            MatchCollection matches = re.Matches(html);
            string carList = "";
            //MessageBox.Show(matches3.Count.ToString());
            System.Collections.IEnumerator enu = matches.GetEnumerator();
            while (enu.MoveNext() && enu.Current != null)
            {
                Match match = (Match)(enu.Current);
                string element = match.Value.Substring(6);
                carList = carList + element.Substring(0, element.Length - 1) + "=";
            }
            if (carList == "") return;
            carList = carList.Substring(0, carList.Length - 1);
            string[] orders = carList.Split('=');
            for (int i = 0; i < orders.Length; i++)
            {
                string order = orders[i].Substring(8);
                order = order.Substring(0, order.Length - 7);
                string url = "http://data.cma.cn/order/goodsDel/storageType/0.html?goodsId=" + order;
                doGet(url, sc, "utf-8", "http://data.cma.cn/order/carView.html");
            }
        }

        private string Login(TestCookie sc)
        {
            string poststr = "userName=" + this.tUsername.Text.Trim() + "&password=" + this.tPassword.Text.Trim() + "&verifyCode=" + this.tCode.Text.Trim();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(poststr);
            string response = doPost("http://data.cma.cn/user/Login.html", data, sc, "utf-8", "http://data.cma.cn/site/index.html");
            return response;
        }
        private void bSubmit_Click(object sender, EventArgs e)
        {
            if (this.dateTimePicker1.Value > this.dateTimePicker2.Value)
            {
                MessageBox.Show("时间选择有误，请重新填写！");
                return;
            }
            if (selectedProv.Equals(""))
            {
                MessageBox.Show("尚未选择省份！");
                return;
            }
            string response = this.Login(sc);
            //MessageBox.Show(response);
            if (response.Split(',')[0].Substring(10) != "100")
            {
                MessageBox.Show("登录信息有误，请重新输入！");
                return;
            }
            string tStart = this.dateTimePicker1.Value.ToString("yyyy-MM-dd 00时");
            string tEnd = this.dateTimePicker2.Value.ToString("yyyy-MM-dd 23时");
            this.textBox1.AppendText(System.Environment.NewLine + "时间:" + tStart + "~" + tEnd);
            this.textBox1.AppendText(System.Environment.NewLine+"登录成功！.......");
            this.resetCarview(sc);
            string response3="";
            string[] provinces = selectedProv.Split(',');
            for (int i = 0; i < provinces.Length; i++)
            {
                response3 = this.submitOrder(provinces[i], sc);
            }
            this.textBox1.AppendText(System.Environment.NewLine + "加入数据筐成功！.......");    
            string regex3 = "name=\"carlist\\[(\\w+)\\]\\[list\\]\"";
            Regex re3 = new Regex(regex3);
            MatchCollection matches3 = re3.Matches(response3);
            string carList = "";
            //MessageBox.Show(matches3.Count.ToString());
            System.Collections.IEnumerator enu = matches3.GetEnumerator();
            while (enu.MoveNext() && enu.Current != null)
            {
                Match match = (Match)(enu.Current);
                string element = match.Value.Substring(6);
                carList = carList + element.Substring(0, element.Length - 1) + "=&";
            }
            carList = carList.Substring(0, carList.Length - 1);
//            this.textBox1.AppendText(carList);
            string submitURL = "http://data.cma.cn/order/createOrder.html?" + carList;
            string submitRE = doGet(submitURL, sc, "utf-8", "http://data.cma.cn/order/carView.html");
            this.textBox1.AppendText(System.Environment.NewLine + "提交订单！......."); 

            JObject jo = (JObject)JsonConvert.DeserializeObject(submitRE);
            string status = jo["status"].ToString();
            string message = jo["data"]["message"].ToString();
            //MessageBox.Show(status + ":" + message);
            this.textBox1.AppendText(System.Environment.NewLine + "状态：" + status + ":" + message);
            if (status == "False") return;
            
            //下载订单
            Thread th = new Thread(delegate()
            {
                int count = 0;
                while (downloadOrder(sc) != true)
                {
                    this.textBox1.AppendText(System.Environment.NewLine + count.ToString() + "min:数据尚未处理完成，程序正在运行，请勿关闭！");
                    Thread.Sleep(1000 * 60 * 1);
                    count++;
                }
                this.textBox1.AppendText(System.Environment.NewLine + "下载完成！");
            });
            th.Start();
        }
        private void StartDownload(string URL,string filepath)
        {
            System.Net.WebClient client = new System.Net.WebClient();
            Stream stream = client.OpenRead(URL);
            StreamReader reader = new StreamReader(stream);
            FileStream outputStream = new FileStream(filepath, FileMode.Create);
            try
            {
                WebRequest myre = WebRequest.Create(URL);
                int bufferSize = 100;
                int nRealCount;
                byte[] bBuffer = new byte[bufferSize];
                nRealCount = stream.Read(bBuffer, 0, bufferSize);
                while (nRealCount > 0)
                {
                    outputStream.Write(bBuffer, 0, nRealCount);
                    nRealCount = stream.Read(bBuffer, 0, bBuffer.Length);
                }
            }
            catch (WebException exp)
            {
                MessageBox.Show(exp.Message, "Error");
            }
            finally
            {
                stream.Close();
                reader.Close();
                outputStream.Close();
            }
        }
        private bool downloadOrder(TestCookie sc)
        {
//             string response = this.Login(sc);
//             if (response.Split(',')[0].Substring(10) != "100")
//             {
//                 MessageBox.Show("登录失败！");
//                 return false;
//             }
            string html = doGet("http://data.cma.cn/order/list/type/online.html", sc, "utf-8", "http://data.cma.cn/order/list/type/online.html");
//             string regex = "<div class=\"item1218 list1218 clearfix\">([^>]*>)([^>]*>)([^>]*>)([^>]*>)([^>]*>)([^>]*>)([^>]*>)([^>]*>)([^>]*>)([^>]*>)";
//             Regex re = new Regex(regex,RegexOptions.Singleline);
//             MatchCollection matches = re.Matches(html);
//             MessageBox.Show(matches.Count.ToString());

            int index = 0;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode rootNode = doc.DocumentNode;
            HtmlNodeCollection collection1 = rootNode.SelectNodes("//div[starts-with(@class,'item1218 list1218 clearfix')]");
            //MessageBox.Show(collection1.Count.ToString());
            string orderInfoDiv = collection1[index].OuterHtml;
            doc.LoadHtml(orderInfoDiv);
            HtmlNodeCollection collection2 = doc.DocumentNode.SelectNodes("//div[starts-with(@class,'progress1218')]");
            string status = collection2[0].OuterHtml.Substring(26);
            status = status.Substring(0, status.Length - 6).Trim();
            HtmlNodeCollection collectionID = doc.DocumentNode.SelectNodes("//div[starts-with(@class,'num1218')]");
            string orderID = collectionID[0].OuterHtml.Substring(46);
            orderID=orderID.Substring(0, orderID.Length - 6).Trim();
            string date = DateTime.Now.ToString("yyyyMMdd");
            //MessageBox.Show(orderID);
            string path = this.tPath.Text + "\\" + orderID;
            if (status == "订单完成-成功")
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                this.textBox1.AppendText(System.Environment.NewLine + "订单号:" + orderID);
                doc.LoadHtml(html);
                HtmlNodeCollection collection3 = doc.DocumentNode.SelectNodes("//table[starts-with(@class,'table data-list-table')]");
                string orderTable = collection3[index].OuterHtml;
                doc.LoadHtml(orderTable);
                HtmlNodeCollection collection4 = doc.DocumentNode.SelectNodes("//a[starts-with(@class,'down1218')]");
                //string url = collection4[0].GetAttributeValue("href",null);
                //string[] provinces=selectedProv.Split(',');
                for (int i = 0; i < collection4.Count; i++)
                {
                    string url = collection4[i].GetAttributeValue("href", null);
                    //MessageBox.Show(url);
                    string regex = "S\\d{4}[01]\\d[0123]\\d[012]\\d[0-5]\\d[0-5]\\d\\d{7}.zip";
                    Regex re = new Regex(regex);
                    MatchCollection matches = re.Matches(url);
                    string filename=matches[0].ToString();
                    //this.StartDownload(url, filename);
                    //if (selectedProv != "") filename = filename.Substring(0, filename.Length - 4) + provinces[i]+".zip";
                    Thread th = new Thread(delegate() { StartDownload(url, path+"\\"+filename); });
                    th.Start();
                }
                using (FileStream fs = new FileStream(path + "\\readme.txt", FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(this.textBox1.Text);
                    sw.Close();
                }
                return true;
//                     using (FileStream fs = new FileStream("F:\\a.html", FileMode.Create))
//                     {
//                         StreamWriter sw = new StreamWriter(fs);
//                         sw.Write(url);
//                         sw.Close();
//                     }
            }
            else return false;
        }
        private void bReset_Click(object sender, EventArgs e)
        {
            selectedProv = "";            
            string response = this.Login(sc);
            if (response.Split(',')[0].Substring(10) != "100")
            {
                MessageBox.Show("登录信息有误，请重新输入！");
                return;
            }
            this.textBox1.Text = "登录成功！开始下载最近订单......";
            //this.downloadOrder(sc);
            Thread th = new Thread(delegate() {
                int count = 0;
                while (downloadOrder(sc) != true) 
                {
                    this.textBox1.AppendText(System.Environment.NewLine + count.ToString()+"min:数据尚未处理完成，程序正在后台运行，请勿关闭！......");
                    Thread.Sleep(1000 * 60);
                    count++;
                }
                this.textBox1.AppendText(System.Environment.NewLine+"下载完成！");
                Thread.CurrentThread.Abort();
            });
            th.Start();
        }
        string cookie = "";
        //private string html = "";
        private void bGetImg_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image = doGetImg("http://data.cma.cn/site/captcha/v/56e2ed093f98f.html", sc);
            //MessageBox.Show(Http.GetHtml("http://data.cma.cn/site/captcha/refresh/1.html"));
            string html = Http.GetHtml("http://data.cma.cn/site/captcha/refresh/1.html",out cookie);
            //MessageBox.Show(cookie);
            
            string[] Array1 = html.Split(',');
            string[] Array2 = Array1[2].Split(':');
            string sPart = Array2[1].Substring(1, Array2[1].Length-3);
            string[] urls = sPart.Split('\\');
            
            string url = "http://data.cma.cn" + urls[1] + urls[2] + urls[3] + urls[4];
            //MessageBox.Show(url);
            pictureBox1.Image = doGetImg(url, sc);    
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.dateTimePicker2.Value = DateTime.Now.AddDays(-1);
            this.dateTimePicker1.Value = this.dateTimePicker2.Value.AddDays(-5);
            this.dateTimePicker2.MaxDate = this.dateTimePicker2.Value;
            this.dateTimePicker2.MinDate = this.dateTimePicker1.Value;
            this.dateTimePicker1.MinDate = this.dateTimePicker1.Value;
            this.dateTimePicker1.MaxDate = this.dateTimePicker2.Value;
            this.bGetImg_Click(sender, e);
            this.textBox1.Text = "所选省份：北京，天津，河北，山东，上海，江苏，浙江，福建，广东";
            selectedProv = this.radioButton1.Tag.ToString();
            //this.tPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Personal)+"\\WeatherData";
            this.tPath.Text = "F:\\Data\\WeatherData";
            if (!Directory.Exists(this.tPath.Text))
            {
                Directory.CreateDirectory(this.tPath.Text);
            }
        }

        private void radioButton5_MouseClick(object sender, MouseEventArgs e)
        {
            Form2 pForm = new Form2();
            pForm.Owner = this;
            pForm.ShowDialog();
            string txt = "";
            if (pForm.DialogResult == DialogResult.OK)
            {
                txt = "所选省份："+pForm.getProvinces();
                this.textBox1.Text = txt;
                selectedProv = pForm.getProvinceIDs();
                //MessageBox.Show(selectedProv);
            }
            else if (pForm.DialogResult == DialogResult.Cancel)
            {
                txt = "所选省份：北京";
                this.textBox1.Text = txt;
                selectedProv = pForm.getProvinceIDs();
                //MessageBox.Show(selectedProv);
            }
        }

        private void radioButton1_MouseClick(object sender, MouseEventArgs e)
        {
            string txt = "所选省份：北京，天津，河北，山东，上海，江苏，浙江，福建，广东";
            this.textBox1.Text = txt;
            selectedProv = this.radioButton1.Tag.ToString();
        }

        private void radioButton2_MouseClick(object sender, MouseEventArgs e)
        {
            string txt = "所选省份：河南，山西，内蒙古，辽宁，吉林，黑龙江，陕西，甘肃，宁夏";
            this.textBox1.Text = txt;
            selectedProv = this.radioButton2.Tag.ToString();
        }

        private void radioButton4_MouseClick(object sender, MouseEventArgs e)
        {
            string txt = "所选省份：安徽，江西，湖北，湖南，广西，海南，重庆，贵州，云南，四川";
            this.textBox1.Text = txt;
            selectedProv = this.radioButton4.Tag.ToString();
        }

        private void radioButton3_MouseClick(object sender, MouseEventArgs e)
        {
            string txt = "所选省份：西藏，青海，新疆，台湾，香港，澳门，极地";
            this.textBox1.Text = txt;
            selectedProv = this.radioButton3.Tag.ToString();
        }

        private void bPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.tPath.Text = dialog.SelectedPath;
            }
            else return;
        }
    }
}
