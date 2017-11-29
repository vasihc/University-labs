using System;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

public class UdpFileClient
{
    // Детали файла
    [Serializable]
    public class FileDetails
    {
        public string FILETYPE = "";
        public long FILESIZE = 0;
    }

    private static FileDetails fileDet;

    // Поля, связанные с UdpClient
    private static int localPort = 5002;

    private static UdpClient receivingUdpClient = new UdpClient(localPort);
    private static IPEndPoint RemoteIpEndPoint = null;

    private static FileStream fs;
    private static Byte[] receiveBytes = new Byte[0];

    [STAThread]
    static void Main(string[] args)
    {
        // Получаем информацию о файле
        GetFileDetails();

        // Получаем файл
        ReceiveFile();
    }

    private static void GetFileDetails()
    {
        try
        {
            Console.WriteLine("-----------*******Waiting info about file*******-----------");

            // Получаем информацию о файле
            receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
            Console.WriteLine("----Information about file was get!");

            XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
            MemoryStream stream1 = new MemoryStream();

            // Считываем информацию о файле
            stream1.Write(receiveBytes, 0, receiveBytes.Length);
            stream1.Position = 0;

            // Вызываем метод Deserialize
            fileDet = (FileDetails) fileSerializer.Deserialize(stream1);
            Console.WriteLine("File got as" + fileDet.FILETYPE +
                              " have size " + fileDet.FILESIZE.ToString() + " byte");
        }
        catch (Exception eR)
        {
            Console.WriteLine(eR.ToString());
        }
    }

    public static void ReceiveFile()
    {
        try
        {
            Console.WriteLine("-----------*******Waiiting to get file*******-----------");

            // Получаем файл
            receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);

            // Преобразуем и отображаем данные
            Console.WriteLine("----File got...Saving...");

            // Создаем временный файл с полученным расширением
            fs = new FileStream("temp." + fileDet.FILETYPE, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs.Write(receiveBytes, 0, receiveBytes.Length);

            Console.WriteLine("----File saved...");

            Console.WriteLine("-------Oppening file------");

            // Открываем файл связанной с ним программой
            Process.Start(fs.Name);
        }
        catch (Exception eR)
        {
            Console.WriteLine(eR.ToString());
        }
        finally
        {
            fs.Close();
            receivingUdpClient.Close();
            Console.Read();
        }
    }
}