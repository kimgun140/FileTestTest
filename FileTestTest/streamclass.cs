using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FileTestTest
{
    public class streamclass
    {


        public void WriteFile(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine("테스트 파일입니다.");
                    sw.WriteLine("FileStream 클래스로 작성");
                }
                MessageBox.Show("작업 완료");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void BinaryWriteFile(string filePath)
        {

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (BinaryWriter bw = new BinaryWriter(fs))
            // 데이터 기록은 한줄씩 되는게 아니라 순서대로 연속적으로 기록이 된다.
            // 
            {
                //bw.Write(filePath);
                bw.Write(42);
                bw.Write(3.14);
                bw.Write(true);
                bw.Write("Hello");
                bw.Flush();


            }


        }
        public void BinaryReadFile(string Filepath)
        {
            using (FileStream fs = new FileStream(Filepath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                int number = br.ReadInt32();
                double pi = br.ReadDouble();
                bool flag = br.ReadBoolean();
                string text = br.ReadString();
                // int를 읽으라고해서 인트의 길이만큼 읽어오고 거기까지 읽은 표시를 남기고 그다음에 읽어도록 되어있는거네 뭔가 대단한게 있는게 아니라 

                MessageBox.Show($"{number},{pi},{flag}, {text}");
            }
        }

        public void ByteWriteReader(string filepath)
        {
            byte[] datatowrite = new byte[1024];//초기화
            new Random().NextBytes(datatowrite);// 이거 뭐지 무작위 숫자로 채우는거네 

            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(datatowrite, 0, datatowrite.Length);// 파일포인터가 이동함 
                //fs.Write(datatowrite);// 뭘까 얘는 좋은거라고 하는데 이해가 안되넹?
                MessageBox.Show("작성완료");
            }

            byte[] dataRead = new byte[1024];

            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(dataRead, 0, dataRead.Length);
                MessageBox.Show($"읽은 데이터{CompareArrays(datatowrite, dataRead)}");
            }

        }
        public bool CompareArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;

            }
            return true;
        }


        public void filecopytest(string sourcepath, string sourthpath2)
        {
            string destinationPath = sourthpath2 + "testdestination.bin";// 경로를 깡으로 적으니까 공유스토리지에 안되었네 
            File.Copy(sourcepath, destinationPath, true); // true는 덮어쓰기 허용 file.copy는 파일 전체를한번에 복사해버림 그리고 파일 내에서 파일포인터 통제도 되안됨 그래서 share access도 종제안됨 
            //copy는 fileAccess FileShare같은 옵션이 없네 그래서 다른 프로세스스에서 작업중일 경우에는 잠겨버리네 접근을 못하게 해버림 그래서 파일이 열려있는 상태인지 잠겨 있는지도 확인해야겠네 
            byte[] dataRead = new byte[1024];//버퍼크기 
            int bytesRead;// read가 반환하는 값 0 나오면 파일 끝까지 읽은거임 
            // 
            using (FileStream fs = new FileStream(destinationPath, FileMode.Open, FileAccess.Read)) //읽기 
            {
                while ((bytesRead = fs.Read(dataRead, 0, destinationPath.Length)) > 0) //파일포인터? 그게 알아서 위치이동하네 
                {

                    MessageBox.Show($"읽은 데이터{dataRead}");
                }
            }
        }

        public async void doublestream(string sourcePath, string destinationPath)
        //파일 읽는 스트림 , 쓰는 스트림 2개 스트림 2개를 열어서 하나는 버퍼크기만큼 읽어오고 다른 하나는 쓰는거구나 
        {
            // 스트림 2개 사용해서 
            int retryCount = 3;// 재시도 

            while (retryCount > 0)// 여기서 재시도해
            {
                using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))// 읽기
                using (FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write)) // 쓰기 
                {
                    try
                    {
                        byte[] buffer = new byte[4096]; // 4KB 버퍼
                        int bytesRead;

                        while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await destinationStream.WriteAsync(buffer, 0, bytesRead);
                        }
                        return;// 완료되면 리떤
                    }
                    catch (IOException ex)
                    {
                        retryCount--;
                        if (retryCount > 0)
                        {
                            await Task.Delay(100);// 잠시 대기 
                        }
                        else
                        {
                            MessageBox.Show($"실패!!{ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
        public void CopyTest(string sourcePath, string destinationPath)
            // copy로 오디오파일 복사하기 
        {
            File.Copy(sourcePath, destinationPath, true);
        }
    }
}
