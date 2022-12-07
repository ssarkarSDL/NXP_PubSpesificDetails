using NXP3_GetPublicationDetails.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace NXP3_GetPublicationDetails
{
    class exportPublicationSpesificDetails
    {
        //public static string csv_Export_Publication_specific_Information_localFilePath = @"C:\temp\doc_logs\outputFiles\0001_CSV\";
        public async static Task Run(List<XmlElement> PubSpesificDetails, InfoShareWSHelper _WSHelper)
        {
            var s = PubSpesificDetails;
            var objectClient25 = _WSHelper.GetDocumentObj25Channel();
            var pubObj = _WSHelper.GetPublication25Channel();
            string logicalID = string.Empty;
            string ftitle = string.Empty;
            string version = string.Empty;
            string fishpubsourcelanguages = string.Empty;
            string fishbaselineID = string.Empty;
            string fishbaselineName = string.Empty;
            string fishresources = string.Empty;
            string fishmasterref = string.Empty;
            

            string text = string.Empty;
            using (var streamReader = new StreamReader(@"C:\temp\doc_logs\inputPubFile.xml", Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
            }
            XmlDocument xDoc1 = new XmlDocument();
            xDoc1.LoadXml(text);
            XmlElement root1 = xDoc1.DocumentElement;
            XmlNodeList nodes1 = root1.SelectNodes("ishobject");
            foreach (XmlElement node in nodes1)
            {
                logicalID = node.Attributes["ishref"].InnerText.ToString();

                foreach (XmlElement subChildNode in node.ChildNodes)
                {
                    foreach (XmlElement childNode in subChildNode.ChildNodes)
                    {
                        if (childNode.Attributes["name"].InnerText.ToString().ToLower().Equals("ftitle"))
                        {
                            ftitle = childNode.InnerText.ToString().Trim().Replace(@"\r","").Replace(@"\n","").Replace(@"\r\n", "").Replace(@"\n\r", "").Replace(@"\t", " ").Replace(@" ", " ");
                            ftitle=string.Join(" ", Regex.Split(ftitle, @"(?:\r\n|\n|\r)"));
                            
                                //ftitle = ftitle.Replace('.', '_').Replace('<', '_').Replace('>', '_').Replace('[', '_').Replace(']', '_').Replace(':', '_').Replace('~', '_').Replace('!', '_').Replace('@', '_').Replace('#', '_').Replace('$', '_').Replace('%', '_').Replace('^', '_').Replace('&', '_').Replace('*', '_').Replace('-', '_').Replace('\\', '_').Replace('/', '_').Replace('=', '_').Replace('(', '_').Replace(')', '_');
                            //ftitle = Regex.Replace(ftitle, @"[^a-zA-Z0-9 ]", "_");
                            //regex
                        }
                        if (childNode.Attributes["name"].InnerText.ToString().ToLower().Equals("version"))
                        {
                            version = childNode.InnerText.ToString().Trim();
                        }
                        if (childNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishmasterref"))
                        {
                            fishmasterref = childNode.InnerText.ToString().Trim();
                        }
                        if (childNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishresources"))
                        {
                            fishresources = childNode.InnerText.ToString().Trim();
                        }
                        if (childNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishpubsourcelanguages"))//FISHPUBSOURCELANGUAGES
                        {
                            fishpubsourcelanguages = childNode.InnerText.ToString().Trim().Replace(@"\r", "").Replace(@"\n", "").Replace(@"\t", " ").Replace(@" ", " ");
                        }
                        if (childNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishbaseline") && childNode.Attributes["ishvaluetype"].InnerText.ToString().ToLower().Equals("element"))//FISHBASELINE
                        {
                            fishbaselineID = childNode.InnerText.ToString().Trim().Replace(@"\r", "").Replace(@"\n", "").Replace(@"\t", " ").Replace(@" ", " ");
                            fishbaselineID = string.Join(" ", Regex.Split(fishbaselineID, @"(?:\r\n|\n|\r)"));
                        }
                        if (childNode.Attributes["name"].InnerText.ToString().ToLower().Equals("fishbaseline") && childNode.Attributes["ishvaluetype"].InnerText.ToString().ToLower().Equals("value"))//FISHBASELINE
                        {
                            fishbaselineName = childNode.InnerText.ToString().Trim().Replace(@"\r", "").Replace(@"\n", "").Replace(@"\t", " ").Replace(@"	", " ");
                            fishbaselineName = string.Join(" ", Regex.Split(fishbaselineName, @"(?:\r\n|\n|\r)"));
                        }

                    }
                }
                //if (ftitle.Length > 30)
                //{
                //    ftitle = ftitle.Substring(0, 29);
                //}
                string csvFileName = Regex.Replace(ftitle, @"[^a-zA-Z0-9 .]", "_") + "=" + logicalID.Trim() + "=" + version.Trim() + "=" + fishpubsourcelanguages.Trim() + ".csv";
                //csvFileName = csvFileName.Substring(0,30)+ ".csv";
                string subFolderName = logicalID.Split('-')[1].Substring(0, 3);
                var newLine = string.Format("Object GUID\tFTITLE\tVersion\tLanguage\tType\tRepository Path\tLocal Path");
                string csv_Export_Publication_specific_Information_localFilePath = @"C:\temp\doc_logs\outputFiles\TDRepo\_Publications\" + subFolderName + @"\" + csvFileName;
                string filepath = string.Empty;
                if (Directory.Exists(@"C:\temp\doc_logs\outputFiles\TDRepo\_Publications\" + subFolderName + @"\"))
                {
                    FileStream fs = File.Create(csv_Export_Publication_specific_Information_localFilePath);
                    filepath = csv_Export_Publication_specific_Information_localFilePath;
                    using (var sr = new StreamWriter(fs))
                    {
                        await sr.WriteLineAsync(newLine);
                        sr.Close();
                        sr.Dispose();
                    }
                }
                else
                {
                    DirectoryInfo tempFolder = new DirectoryInfo(@"C:\temp\doc_logs\outputFiles\TDRepo\_Publications");
                    DirectoryInfo subFolder = tempFolder.CreateSubdirectory(subFolderName);
                    csv_Export_Publication_specific_Information_localFilePath = @"C:\temp\doc_logs\outputFiles\TDRepo\_Publications\" + subFolderName + @"\" + csvFileName;
                    FileStream fs = File.Create(csv_Export_Publication_specific_Information_localFilePath);
                    filepath = csv_Export_Publication_specific_Information_localFilePath;
                    using (var sr = new StreamWriter(fs))
                    {
                        await sr.WriteLineAsync(newLine);
                        sr.Close();
                        sr.Dispose();
                    }
                }
                Console.WriteLine("pubid: " + fishbaselineID);
                Console.WriteLine("folder path: " + csv_Export_Publication_specific_Information_localFilePath);
                getUsedObjectsFromBaseLineName(fishpubsourcelanguages, fishbaselineID, _WSHelper, filepath, csvFileName, logicalID, fishmasterref, fishresources);

            }
            
        }

       
        public static void getUsedObjectsFromBaseLineName(string lang,string baselineID, InfoShareWSHelper _WSHelper, string filepath, string csvFileName, string pubID, string fishmasterref, string fishresources)
        {
            var baseline25Obj = _WSHelper.GetBaseline25Channel();
            //string[] langu = new string[1];
            //langu[0] = lang;
            //string[] id = new string[1];
            //id[0] = baselineID;
            //string[] res = new string[2];
            //res[0] = "low";
            //res[1] = "high";
            string xmlBaseLine = string.Empty;
            //string xmlRequestedMetadata4BaseLine = "<ishfields>" +
            //                    "<ishfield name='FTITLE' level='lng'/>" +
            //                    "<ishfield name='DOC-LANGUAGE' level='lng'/>" +
            //                    "<ishfield name='FRESOLUTION' level='lng'/>" +
            //                "</ishfields>";
            try
            {
                //  string sID = baseline25Obj.GetReport(baselineID,null, langu, null, null,res);
                //string sID = baseline25Obj.RetrieveMetadata2(id, Baseline25ServiceReference.ActivityFilter.None, null, null);
                var objectClient25 = _WSHelper.GetDocumentObj25Channel();

                string usedObjectOutput = baseline25Obj.ExpandBaseline(baselineID, new string[] { fishmasterref },new string[] { fishresources }, new string[]{lang}, null, null, new string[] { "high" });
                //baseline25Obj.CleanUp(baselineID);
               // string usedObjectOutput = baseline25Obj.GetBaseline(out xmlBaseLine, baselineID, null);
                //string s = xmlBaseLine;
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(usedObjectOutput);
                XmlElement root = xDoc.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("//objects");/**/
                //string baseLineIshlogicalref = pubid;
                string baseLineIshversionref = string.Empty;
                string baseLineIshGUID = string.Empty;
                if (nodes != null)
                {
                    foreach (XmlElement subnodes in nodes)
                    {

                        foreach (XmlElement node in subnodes)
                        {
                            if (node.HasAttribute("versionnumber")&& node.HasAttribute("ref"))
                            {
                                if (node.HasAttribute("versionnumber"))
                                {
                                    baseLineIshversionref = node.Attributes["versionnumber"].InnerText.ToString();
                                }
                                if (node.HasAttribute("ref"))
                                {
                                    baseLineIshGUID = node.Attributes["ref"].InnerText;
                                }
                                //https://nxpdev001.sdlproducts.com/ISHWS/Wcf/API25/DocumentObj.svc
                                string[] ids = new string[1];
                                ids[0] = baseLineIshGUID;

                                string xmlRequestedMetadata = "<ishfields>" +
                                    "<ishfield name='FTITLE' level='logical'/>" +
                                    "<ishfield name='DOC-LANGUAGE' level='lng'/>" +
                                    "<ishfield name='FRESOLUTION' level='lng'/>" +
                                "</ishfields>";
                                string requestedMetadata = objectClient25.RetrieveMetadata(ids, DocumentObj25ServiceReference.StatusFilter.ISHNoStatusFilter, null, xmlRequestedMetadata);

                                XmlDocument xDoc1 = new XmlDocument();
                                xDoc1.LoadXml(requestedMetadata);
                                XmlElement root1 = xDoc1.DocumentElement;
                                XmlNodeList nodes1 = root1.SelectNodes("ishobject");
                                string ishlngref1 = string.Empty;
                                string FTITLE1 = string.Empty;
                                string FRESOLUTION = string.Empty;
                                string rawtype = string.Empty;
                                string type = string.Empty;

                                if (nodes1 != null)
                                {
                                    foreach (XmlElement node1 in nodes1)
                                    {
                                        rawtype = node1.Attributes["ishtype"].InnerText.ToString();
                                        type = GetObjectFriendlyType(rawtype);
                                        foreach (XmlElement childNode1 in node1.ChildNodes)
                                        {
                                            foreach (XmlElement subChildNode1 in childNode1.ChildNodes)
                                            {
                                                if (subChildNode1.Attributes["name"].InnerText.ToString().ToLower().Equals("doc-language"))
                                                {
                                                    ishlngref1 = subChildNode1.InnerText.ToString();
                                                }
                                                if (subChildNode1.Attributes["name"].InnerText.ToString().ToLower().Equals("ftitle"))
                                                {
                                                    FTITLE1 = subChildNode1.InnerText.ToString().Trim().Replace(@"\r", "").Replace(@"\n", "").Replace(@"\r\n", "").Replace(@"\n\r", "").Replace(@"\t", " ").Replace(@"  ", " ");
                                                    //FTITLE1 = childNode.InnerText.ToString().Trim().Replace(@"\r", "").Replace(@"\n", "").Replace(@"\r\n", "").Replace(@"\n\r", "");
                                                    FTITLE1 = string.Join(" ", Regex.Split(FTITLE1, @"(?:\r\n|\n|\r)"));

                                                    //FTITLE1 = FTITLE1.Replace('.', '_').Replace('<', '_').Replace('>', '_').Replace('[', '_').Replace(']', '_').Replace(':', '_').Replace('~', '_').Replace('!', '_').Replace('@', '_').Replace('#', '_').Replace('$', '_').Replace('%', '_').Replace('^', '_').Replace('&', '_').Replace('*', '_').Replace('-', '_').Replace('\\', '_').Replace('/', '_').Replace('=', '_').Replace('(', '_').Replace(')', '_');
                                                    // FTITLE1 = Regex.Replace(FTITLE1, @"[^a-zA-Z0-9 ]", "_");

                                                }
                                                if (subChildNode1.Attributes["name"].InnerText.ToString().ToLower().Equals("fresolution"))
                                                {
                                                    FRESOLUTION = subChildNode1.InnerText.ToString();
                                                }
                                            }
                                        }
                                    }
                                }

                                string[] objDocsFolderLocation = new string[] { };
                                long[] objDocsFolderID = new long[] { };
                                objectClient25.FolderLocation(out objDocsFolderLocation, out objDocsFolderID, baseLineIshGUID);
                                string folderPath = string.Join("/", objDocsFolderLocation);
                                //folderpath
                                if (folderPath.ToLowerInvariant().StartsWith("condition management") || folderPath.ToLowerInvariant().StartsWith("editor templates") || folderPath.ToLowerInvariant().StartsWith("publishing") || folderPath.ToLowerInvariant().StartsWith("synchronizer"))
                                {
                                    folderPath = "System/" + folderPath;
                                }
                                else
                                {
                                    folderPath = "NXP_Prod/" + folderPath;
                                }
                                //string filepath = csvFileName;
                                writetoFile(baseLineIshGUID, FTITLE1, baseLineIshversionref, ishlngref1, type, folderPath, filepath, csvFileName, FRESOLUTION);

                            }
                    }
                    
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("exception: " + e.Message);
            }
        }

        public static void writetoFile(string baseLineIshGUID, string FTITLE1, string baseLineIshversionref, string ishlngref1, string type, string folderPath, string filepath, string csvFileName, string FRESOLUTION)
        {
            //string localFilePath = string.Empty;
            string [] arrtype = folderPath.Split('/');
            //string type = arrtype[arrtype.Length - 1];
           // string objFilename = FTITLE1 + "=" + baseLineIshGUID + "=" + baseLineIshversionref + "=" + ishlngref1 +".xml";
            //string localFilePath = @"C:\temp\doc_logs\outputFiles" + @"\" + baseLineIshGUID.Split('-')[1].Substring(0, 3) + @"\" + csvFileName;
            var objfolderID= baseLineIshGUID.Trim().Split('-')[1].Substring(0, 3);
            string ft = string.Empty;
            string fileExtension = ".xml";
            if(FTITLE1.Contains('.'))
            {
                ft = FTITLE1.Split('.')[0].Trim();
                ft = Regex.Replace(ft, @"[^a-zA-Z0-9 .]", "_");
                fileExtension = FTITLE1.Split('.')[1].Trim();
            }
            else
            {
               ft= Regex.Replace(FTITLE1, @"[^a-zA-Z0-9 .]", "_");
                fileExtension = ".xml";
            }
            var objFileNAme= ft + "=" + baseLineIshGUID + "=" + baseLineIshversionref + "=" + ishlngref1 + "=" + FRESOLUTION + fileExtension;
            var directoryName = @"C:\temp\doc_logs\outputFiles\TDRepo\" + objfolderID+@"\"+ objFileNAme;
            var directoryName1 = @"C:\temp\doc_logs\outputFiles\TDRepo\" + objfolderID+@"_1\"+ objFileNAme;
            string objectfilepath = "";
            if(File.Exists(directoryName))
            {
                objectfilepath = directoryName;
            }
            else if (File.Exists(directoryName1))
            {
                objectfilepath = directoryName1;
            }
            else
            {
                objectfilepath = "file not found";
            }
            string reportLocalfilepath = objectfilepath.Replace(@"C:\temp\doc_logs\outputFiles\", @".\");

            var newContent = string.Format("{0}{1}{2}{3}{4}{5}{6}", "\"" + baseLineIshGUID.Trim() + "\"\t", "\"" + FTITLE1.Trim() + "\"\t", "\"" + baseLineIshversionref.Trim() + "\"\t", "\"" + ishlngref1.Trim() + "\"\t", "\"" + type.Trim() + "\"\t", "\"" + folderPath + "\"\t", "\"" + reportLocalfilepath.Trim() + "\"");

            if (File.Exists(filepath))
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(newContent);
                    sw.Close();
                    sw.Dispose();
                }
            }
        }
        public static string GetObjectFriendlyType(string rawType)
        {
            switch (rawType)
            {
                case "ISHModule":
                    return "Topic";
                case "ISHIllustration":
                    return "Image";
                case "ISHLibrary":
                    return "Library";
                case "ISHMasterDoc":
                    return "Map";
                case "ISHPublication":
                    return "Publication";
                default:
                    return "Other";
            }
           
        }
    }
}
