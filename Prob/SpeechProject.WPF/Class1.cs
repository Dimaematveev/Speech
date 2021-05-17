using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechProject.WPF
{
    public class UserData
    {
        public string Name;
        public float[] Data;

        public UserData(string name, float[] data)
        {
            Name = name;
            Data = data;
        }
    }

    public class ListUser
    {
        string file;
        List<UserData> userDatas = new List<UserData>();

        public ListUser(string file)
        {
            this.file = file;
            SetFromFile();
        }

        public string AddUserData(string name, float[] data)
        {
            if (userDatas.Any(x=>x.Name==name))
            {
                return $"Пользователь {name} уже имеется";

            }
            userDatas.Add(new UserData(name, data));
            LoadToFile();
            return null;
        }


        public void SetFromFile()
        {
            using (StreamReader streamReader = new StreamReader(file,System.Text.Encoding.Default))
            {
                while (true)
                {

                    string str = streamReader.ReadLine();
                    if (str == null)
                    {
                        return;
                    }
                    var st = str.Split(';');
                    string na = st[0];
                    List<float> fl = new List<float>();
                    for (int i = 1; i < st.Length; i++)
                    {
                        fl.Add(float.Parse(st[i]));
                    }
                    userDatas.Add(new UserData(na, fl.ToArray()));
                }
            }
        }

        public void LoadToFile()
        {
            using (StreamWriter streamWriter = new StreamWriter(file, false, System.Text.Encoding.Default)) 
            {
                
                foreach (var user in userDatas)
                {
                    string str = "";
                    str += user.Name+";";
                    foreach (var item in user.Data)
                    {
                        str += item.ToString() + ";";
                    }
                    str = str.TrimEnd(';');
                    streamWriter.WriteLine(str);
                }
            }
        }

        public UserData Find(float[] fl)
        {
            float od = 0.3f;
            for (int i = 0; i < userDatas.Count; i++)
            {
                bool b = true;
                for (int j = 0; j < userDatas[i].Data.Length; j++)
                {
                    if (Math.Abs( userDatas[i].Data[j] - fl[j]) > od)
                    {
                        b = false;
                        break;
                    }
                }
                if (b == true)
                {
                    return userDatas[i];
                }
            }
            return null;
        }
    }
}
