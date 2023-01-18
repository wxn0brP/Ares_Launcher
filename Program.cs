namespace Ares_Luncher;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Diagnostics;
using System.Net.NetworkInformation;


static class Program{
    [STAThread]
    static void Main(string[] args){
		ApplicationConfiguration.Initialize();
		if(args.Length > 0 && args[0] == "-update"){
			runDll("update.dll", "update.update");
			return;
		}
		if(installAres()) runDll("Ares_lib.dll", "Ares.Program", string.Join("/;/", args));
    }    
	
	public static void runDll(string path, string name, string plusParam=""){
		if(!File.Exists(path)){
			MessageBox.Show("Nie udało się odnaleść pliku który jest wymagany w Projekte Ares. Nzawa pliku: "+path);
			return;
		}
		try{
			byte[] DllData = File.ReadAllBytes(path);
            var DLL = Assembly.Load(DllData);
            Type? theType = DLL.GetType(name);
            if(theType == null) throw new Exception("Get Type null");
            var function = Activator.CreateInstance(theType);
            MethodInfo? method = theType.GetMethod("Init");
            if(method == null) throw new Exception("Get Method null");
            method.Invoke(function, new object[]{plusParam});
		}catch(Exception e){
			MessageBox.Show(
				"Wystąpił problem! \n\ninformacja developerska:\n"+
				e
			);
		}
	}

	public static bool installAres(){
		if(!File.Exists("Ares_lib.dll")){
			if(!ping("projektares.tk")){
				MessageBox.Show("Aby zainstalować Projekt Ares połącz się z Internetem!");
				return false;
			}
			downloadFile("https://projektares.tk/Ares/Ares_lib.dll", "Ares_lib.dll");
			runDll("Ares_lib.dll", "Ares.Program");
			if(File.Exists("Ares_lib.dll") && !File.Exists("instaled.txt")) File.Delete("Ares_lib.dll");
			return false;
		}
		return true;
	}

	public static void downloadFile(string server_file, string path_to_file){
        if(File.Exists(path_to_file)){
            File.Delete(path_to_file);
        }
        WebClient webClient = new WebClient();
        webClient.DownloadFile(server_file, path_to_file);
    }

    public static bool ping(string server){
        try{
            Ping pinger = new Ping();
            PingReply reply = pinger.Send(server);
            return reply.Status == IPStatus.Success;
        }catch{
            return false;
        }
    }
}
