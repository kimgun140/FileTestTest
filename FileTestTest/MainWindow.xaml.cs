using System.IO;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using static System.Net.Mime.MediaTypeNames;

namespace FileTestTest
{
    /// <summary>
//hehe hehe
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("mpr.dll")]
        // Windows API 함수 WNetAddConnection2를 호출하기 위한 선언입니다. 
        // Multiple Provider Router DLL 에서 제공하는 함수, smb 네트워크 드라이브 연결을 설정하는 역할을 한다. netresource 구조체를 전달하여 네트워크 경로를 지정하고, username과 password로 인증을 수행한다. 
        // 스토리지 접근권한 주기
        private static extern int WNetAddConnection2(ref NETRESOURCE netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        // smb 네트워크 드라이브 연결을 해재하는 기능을 한다.
        private static extern int WNetCancelConnection2(string name, int flags, bool force);
        [StructLayout(LayoutKind.Sequential)]// 메모리에 순차적으로 배치하다
        private struct NETRESOURCE
        // 네트워크 리소스 나타내는 구조체 
        {
            public int dwScope;
            public int dwType;//리소스 유형 1 이면 디스크 드라이브 
            public int dwDisplayType;
            public int dwUsage;
            public string lpLocalName;// 로컬드라이브 문자, 매핑할 경우 사용 
            public string lpRemoteName;// 원격 경로
            public string lpComment;
            public string lpProvider;
        }
        string uncPath = @"\\gms-mcc-nas01\AUDIO-FILE\test1\test121.txt";
        //string content = "이것은 공유 스토리지에 저장되는 테스트 파일입니다.";
        string username = "develop";
        string password = "Akds0ft3!48";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //구조체에 연결에 필요한 정보가 있나
            NETRESOURCE netResource = new NETRESOURCE
            {
                dwType = 1,
                lpRemoteName = @"\\gms-mcc-nas01\AUDIO-FILE\test1"
            };
            // 구조체 초기화해 경로 설정 
            int result = WNetAddConnection2(ref netResource, password, username, 0);

            if (result == 0)// 0이면 연결 
            {

                try
                {
                    File.WriteAllText(uncPath + "0217", "이것은 테스트파일입니다."); /// //////
                    MessageBox.Show("테스트");

                    using (StreamWriter sw = new StreamWriter(uncPath, true)) // true: 기존 파일에 추가
                    {
                        sw.WriteLine($"[{DateTime.Now}] SMB 네트워크 드라이브 테스트 로그입니다.");
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
                WNetCancelConnection2(@"\\gms-mcc-nas01\AUDIO-FILE\test1", 0, true);
                //연결해제 lpRemoteName 을 넣어주기  
                //windows 네트워크 드라이브는 네트워크 공유 폴더 단위로 관리되므로, 연결을 해제할때 해당 공유폴더의 경로를 지정한다.
            }
            else
            {

                MessageBox.Show(result.ToString());
            }
        }


        public void ChangingTest()
        {
            NETRESOURCE nETRESOURCE = new NETRESOURCE
            {
                lpRemoteName = @"\\gms-mcc-nas01\AUDIO-FILE\test1",
                dwType = 1,


            };
            int result = WNetAddConnection2(ref nETRESOURCE, password, username, 0);
            try
            {
                string appendText = "\n새로운 내용이 추가되었습니다.";
                File.AppendAllText(uncPath, appendText);
         

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message +$"{result}");
            }


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ChangingTest();
        }
        public void AttibuteChangingtest()
            // 파일속성을 읽기 전용으로 변경해서 수정이 안된다. 다른 메서드로 다시 쓰거나, 변경하려고하면 권한 에러? 변경 안됨 
        {
            NETRESOURCE nETRESOURCE = new NETRESOURCE
            {
                lpRemoteName = @"\\gms-mcc-nas01\AUDIO-FILE\test1",
                dwType = 1,
            };
            int result = WNetAddConnection2( ref nETRESOURCE, password, username, 0);

            try
            {
                File.SetAttributes(uncPath, FileAttributes.Normal);
                MessageBox.Show("파일이 읽기 전용으로 설정되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{result},{ex.Message}");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AttibuteChangingtest();
        }

       public void AuthTest()
            // 파일 권한 및 인증  이거 뭐지 계정의 권한이 없으면 공유 스토리지에 접근 못할
        {
            var unc = @"\\gms-mcc-nas01\AUDIO-FILE\test1";
            NETRESOURCE nETRESOURCE = new NETRESOURCE
            {
                lpRemoteName = unc,
                dwType = 1,

            };
            int result = WNetAddConnection2(ref nETRESOURCE, password, username, 0);
            var fileInfo = new FileInfo(unc);
            MessageBox.Show(fileInfo.FullName);
            var acl = fileInfo.GetAccessControl();
            MessageBox.Show(acl.ToString());
           //MessageBox.Show( acl.GetOwner(typeof(System.Security.Principal.NTAccount)).ToString());

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AuthTest();

        }
        public void retryTest()
        {
            var unc = @"\\gms-mcc-nas01\AUDIO-FILE\test1";
            NETRESOURCE nETRESOURCE = new NETRESOURCE
            {
                lpRemoteName = unc,
                dwType = 1,

            };
            int result = WNetAddConnection2(ref nETRESOURCE, password, username, 0);
            int retryCount = 3;
            unc += $"{ DateTime.Now:yyyyMMddHHmmss}.txt";
            while (retryCount > 0)
            {
                try
                {
                    File.WriteAllText(uncPath, "데이터 저장");
                    Console.WriteLine("파일 저장 성공!");
                    break;
                }
                catch (IOException)
                {
                    retryCount--;
                    Console.WriteLine("파일 접근 실패, 재시도...");
                    System.Threading.Thread.Sleep(1000); // 1초 후 재시도
                }
            }
        }
       public void fsWrite()
        {
            var streamclasss = new streamclass();
            string uncPathtest = @"\\gms-mcc-nas01\AUDIO-FILE\test1\testFilestream.txt";
            streamclasss.WriteFile(uncPathtest);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            fsWrite();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var streamclasss = new streamclass();
            string uncPathtest = @"\\gms-mcc-nas01\AUDIO-FILE\test1\TestBIN.bin";
            streamclasss.BinaryWriteFile(uncPathtest);
            MessageBox.Show("바이너리write");
            streamclasss. BinaryReadFile( uncPathtest );

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var streamclasss = new streamclass();
            string uncPathtest = @"\\gms-mcc-nas01\AUDIO-FILE\test1\bytebintest.bin";
            streamclasss.ByteWriteReader( uncPathtest );
            string uncPathtest2 = @"\\gms-mcc-nas01\AUDIO-FILE\test1\";
            streamclasss.filecopytest(uncPathtest,uncPathtest2);

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            var streamclasss  = new streamclass();
            string uncPathtest = "C:\\Users\\kimgu\\OneDrive\\바탕 화면\\20241204000000_녹음0.wav";
            string uncPathtest2 = @"\\gms-mcc-nas01\AUDIO-FILE\test1\AudioTest2.mp3";
            streamclasss.doublestream(uncPathtest,uncPathtest2);



        }

        private async void Button_Click_8(object sender, RoutedEventArgs e)
        {
            //copyAudioTest
            var streamclasss = new streamclass();
            string uncPathtest = "C:\\Users\\kimgu\\OneDrive\\바탕 화면\\20241204000000_녹음0.wav";
            string uncPathtest2 = @"\\gms-mcc-nas01\AUDIO-FILE\test1\AudioTestCopy123.mp3";

            streamclasss.CopyTest(uncPathtest, uncPathtest2);
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            WNetCancelConnection2(@"\\gms-mcc-nas01\AUDIO-FILE\test1", 0, true);
        }
    }
}