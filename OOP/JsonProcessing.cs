using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;

namespace Learning_System.ExternalClass
{
    /// <summary>
    /// Methods for processing Json file
    /// </summary>
    public class JsonProcessing
    {
        private static StreamReader SetFileJsonInDefaultFolder(string JsonPath)
        {
            StreamReader _fileReader;
            try
            {
                _fileReader = new StreamReader(JsonPath);
            }
            catch
            {
                _fileReader = new StreamReader("../../" + JsonPath);
            }
            return _fileReader;
        }
        private static StreamReader CreateFileJsonInDefaultFolder(string JsonPath, string sampleJsonWebPath, string sampleContent)
        {
            try
            {
                string _fileName = JsonPath;
                if (File.Exists(_fileName))
                    File.Delete(_fileName);

                try
                {
                    var _webRequest = (HttpWebRequest)WebRequest.Create(sampleJsonWebPath);
                    _webRequest.UserAgent = "Simple Calculator";

                    var _response = _webRequest.GetResponse();
                    var _content = _response.GetResponseStream();

                    using (var _reader = new StreamReader(_content))
                    {
                        string _stringContent = _reader.ReadToEnd();
                        byte[] _info = new UTF8Encoding(true).GetBytes(_stringContent);

                        FileStream _fs = File.Create(_fileName);
                        _fs.Write(_info, 0, _info.Length);
                        _fs.Close();
                        DialogResult _userReturnDialog = MessageBox.Show("Can't find your request file in default folders. We created a sample one for you.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch
                {
                    if (sampleContent != null)
                        File.WriteAllText(JsonPath, sampleContent + "\n");
                    else
                    {
                        DialogResult _userReturnDialog = MessageBox.Show("Can't find your request file in default folders. We couldn't create a sample one for you. Please check your internet connection and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (_userReturnDialog == DialogResult.OK)
                        {
                            Application.Exit();
                        }
                    }
                }

                return SetFileJsonInDefaultFolder(JsonPath);
            }
            catch (Exception _ex)
            {
                DialogResult _userReturnDialog = MessageBox.Show("Error in creating Json file! " + _ex, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (_userReturnDialog == DialogResult.OK)
                    Application.Exit();

                return null;
            }
        }
        private static StreamReader GetFileJsonInDefaultFolder(string JsonPath, string sampleJsonWebPath, string sampleContent)
        {
            try
            {
                StreamReader _readFile;
                try
                {
                    _readFile = SetFileJsonInDefaultFolder(JsonPath);
                }
                catch
                {
                    // Can't find Json file in default folder => Create a new Json file
                    _readFile = CreateFileJsonInDefaultFolder(JsonPath, sampleJsonWebPath, sampleContent);
                }

                return _readFile;
            }
            catch
            {
                DialogResult userReturnDialog = MessageBox.Show("Error in getting file information! ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (userReturnDialog == DialogResult.OK)
                    Application.Exit();

                return null;
            }
        }

        /// <summary>
        /// Get content in your request Json file. If file doesn't exist in default folder, we will create a new one (with sample content) for you.
        /// </summary>
        /// <param name="JsonPath">The relative path of your json file. The root folder is where you put for running application file (or bin folder if you are debugging).</param>
        /// <param name="sampleJsonWebPath">The json url you want to import if we need to create a new one for you.</param>
        /// <param name="sampleContent">The content you want to import if we need to create a new one for you (provided that you can't access to your web path)</param>
        /// <returns>Your data in JArray format. If something get error, it will return a null value.</returns>
        public static JArray ImportJsonContentInDefaultFolder(string JsonPath, string sampleJsonWebPath, string sampleContent)
        { 
            try
            {
                using (var _JsonFile = GetFileJsonInDefaultFolder(JsonPath, sampleJsonWebPath, sampleContent))
                {
                    JArray _JsonData = JArray.Parse(_JsonFile.ReadToEnd());
                    return _JsonData;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Add JsonData into Json file. If you don't have Json file in default folder, we will create a new one for you.
        /// </summary>
        /// <param name="JsonPath">The relative path of your json file. The root folder is where you put for running application file (or bin folder if you are debugging).</param>
        /// <param name="JsonData">Your data which you want to import to json file in JArray format.</param>
        /// <returns>Your new data in JArray format. If something get error, it will return a null value.</returns>
        public static JArray ExportJsonContentInDefaultFolder(string JsonPath, JArray JsonData)
        {
            try
            {
                var _convertedJson = JsonData.ToString();

                string _fileName = JsonPath;
                if (File.Exists(_fileName))
                {
                    File.WriteAllText(_fileName, _convertedJson);
                    return ImportJsonContentInDefaultFolder(JsonPath, null, _convertedJson);
                }
                else
                    return ImportJsonContentInDefaultFolder(JsonPath, null, _convertedJson);
            }
            catch
            {
                return null;
            }
        }
    }
}
