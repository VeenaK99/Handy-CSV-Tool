using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace HandyCSVConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string NoSingleQuoteCSV = "NOSINGLEQUOTECSV";

        private const string myCustomDelimiter = "##$$@@##%$##";

        private const String PlainCSV = "PLAINCSV";

        private const string splitFor1000CSV = "SPLITFOR100CSV";
        private void JumpList_JumpItemsRejected(object sender, System.Windows.Shell.JumpItemsRejectedEventArgs e)
        {

        }

        private void JumpList_JumpItemsRemovedByUser(object sender, System.Windows.Shell.JumpItemsRemovedEventArgs e)
        {

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args != null && e.Args.Length > 0)
            {
                ExecuteCSVActions(e.Args.FirstOrDefault());
            }
            else
            {
                JumpList jumpList1 = JumpList.GetJumpList(App.Current);
                jumpList1.JumpItems.Clear();
                var csvConfigSection= ConfigurationManager.GetSection(StringSplitConfigDataSection.SectionName) as StringSplitConfigDataSection;

                if(csvConfigSection != null && csvConfigSection.CsvConfigCollection.Count > 0)
                {
                    foreach (var item in csvConfigSection.CsvConfigCollection)
                    {
                        CSVConfigItem configItem = item as CSVConfigItem;
                        string fieldArgument = configItem.FieldDelimiter + myCustomDelimiter + configItem.EncloseCharacter + myCustomDelimiter + configItem.MaxItemsPerLine;
                        AddJumpListTask(configItem.Name, "", fieldArgument);
                    }
                }
                System.Environment.Exit(0);
                //AddJumpListTask("CSV", "Converts Line seperated text from clipboard into csv", PlainCSV);
                //AddJumpListTask("Split by 1000", "Converts text from clipboard into csv seperated by 1000 chars", splitFor1000CSV);
                //AddJumpListTask("No Single Quote CSV", "Converts Line seperated text from clipboard into csv but without enclosing quote", NoSingleQuoteCSV);
            }

            JumpList jumpList2 = JumpList.GetJumpList(App.Current);
            jumpList2.Apply();
            System.Environment.Exit(0);
        }

        private void AddJumpListTask(string taskName, string description, string argument)
        {
            // Configure a new JumpTask.
            JumpTask jumpTask1 = new JumpTask();
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            jumpTask1.ApplicationPath = path;
            jumpTask1.Title = taskName;
            jumpTask1.Description = description;
            jumpTask1.Arguments = argument;
            JumpList jumpList1 = JumpList.GetJumpList(App.Current);
            jumpList1.JumpItems.Add(jumpTask1);
            JumpList.AddToRecentCategory(jumpTask1);
            jumpList1.Apply();
        }

        public void ExecuteCSVActions(string actionType)
        {
            string inputText = Clipboard.GetText();
            string outPutText = string.Empty;
            switch (actionType.ToUpper())
            {
                case "PLAINCSV":
                    outPutText = this.ConvertToCSV(inputText, ",");
                    break;
                case "SPLITFOR100CSV":
                    outPutText = this.ConvertToCSV(inputText, ",", lengthPerLine: 1000);
                    break;
                case "NOSINGLEQUOTECSV":
                    outPutText = this.ConvertToCSV(inputText, ",", encloseCharacter:"");
                    break;
                default:
                   var allItems= actionType.Split( new string[] { myCustomDelimiter }, StringSplitOptions.None );
                    int maxLines = -1;
                    int.TryParse(allItems[2], out maxLines);
                    outPutText = this.ConvertToCSV(inputText, allItems[0], allItems[1], maxLines);
                    break;
            }

            Clipboard.SetText(outPutText);
        }

        private string ConvertToCSV(string inputText, string delimiter, string encloseCharacter ="'", int lengthPerLine = -1 )
        {
            string[] splitArray = inputText.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string outText = string.Empty;
            if (lengthPerLine != -1)
            {
                int arrayLength = splitArray.Length;
                int skipCount = 0;
                do
                {
                    int takeCount = arrayLength - lengthPerLine > 0 ? lengthPerLine : arrayLength;
                    
                    // conver the first n characters and append a line to it
                    outText+= encloseCharacter + string.Join(encloseCharacter + delimiter + encloseCharacter, 
                        splitArray.Skip(skipCount).Take(takeCount)) + encloseCharacter;

                    outText += Environment.NewLine;

                    arrayLength = arrayLength - lengthPerLine;
                    skipCount += lengthPerLine;

                } while (arrayLength > 0);
            }
            else
                outText =  encloseCharacter + string.Join(encloseCharacter + delimiter + encloseCharacter, splitArray) + encloseCharacter;

            return outText;
        }

        private void OnJumpItemsRemoved(object sender, JumpItemsRemovedEventArgs e)
        {

        }

        private void OnJumpItemsRejected(object sender, JumpItemsRejectedEventArgs e)
        {

        }
    }
}
