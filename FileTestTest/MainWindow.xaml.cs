using System.IO;
using System.Runtime.InteropServices;
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

namespace FileTestTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        [DllImport("mpr.dll")]
        // Windows API 함수 WNetAddConnection2를 호출하기 위한 선언입니다. 
        // Multiple Provider Router DLL 에서 제공하는 함수, smb 네트워크 드라이브 연결을 설정하는 역할을 한다. netresource 구조체를 전달하여 네트워크 경로를 지정하고, username과 password로 인증을 수행한다. 
        // 접근권한 주는구나
        private static extern int WNetAddConnection2(ref NETRESOURCE netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        // smb 네트워크 드라이브 연결을 해재하는 기능을 한다.
        private static extern int WNetCancelConnection2(string name, int flags, bool force);
        [StructLayout(LayoutKind.Sequential)]
        private struct NETRESOURCE
        // 네트워크 리소스 나타내는 구조체 
        {
            public int dwScope;
            public int dwType;
            public int dwDisplayType;
            public int dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string uncPath = @"\\gms-mcc-nas01\AUDIO-FILE\test1\test121.txt";
            //string content = "이것은 공유 스토리지에 저장되는 테스트 파일입니다.";
            string username = "develop";
            string password = "Akds0ft3!48";

            NETRESOURCE netResource = new NETRESOURCE
            {
                dwType = 1,
                lpRemoteName = @"\\gms-mcc-nas01\AUDIO-FILE\test1"
            };
            // 구조체 초기화해 경로 설정 
            int result = WNetAddConnection2(ref netResource, password, username, 0);

            if (result == 0)
            {

                try
                {
                    //File.WriteAllText

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
                //연결해제
            }
            else
            {
                MessageBox.Show(result.ToString());
            }
        }
    }
}