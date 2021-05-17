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
            return null;
        }


        public void SetFromFile()
        {
            using (StreamReader streamReader = new StreamReader(file))
            {
                string str = streamReader.ReadLine();
                if (str==null)
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

                AddUserData(na, fl.ToArray());
            }
        }

        public void GetFromFile()
        {
            using (StreamWriter streamWriter = new StreamWriter(file) )
            {
                string str = "";
                foreach (var user in userDatas)
                {
                    str += user.Name+";";
                    foreach (var item in user.Data)
                    {
                        str += item.ToString() + ";";
                    }
                    streamWriter.WriteLine(str);
                }
            }
        }
    }
}
