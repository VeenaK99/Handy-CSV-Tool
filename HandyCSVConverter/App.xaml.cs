//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="CompanyName">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace HandyCSVConverter
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Windows;
    using System.Windows.Shell;

    /// <summary>
    /// Interaction logic for application, this is the first point from where the application will be launched
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// A custom delimiter to separate the individual parameters for each of the action items
        /// </summary>
        private const string ParameterDelimiter = "__________________";

        /// <summary>
        /// The default argument to be considered in case there are no input arguments. 
        /// This argument would be comma separator
        /// </summary>
        private readonly string commaSeperatorDefault = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            CSVConfigItem item = new CSVConfigItem();
            item.SplitChar = @"\r\n";
            item.FieldDelimiter = ",";
            item.EncloseCharacter = string.Empty;
            item.MaxItemsPerLine = -1;
            this.commaSeperatorDefault = this.GetConcatenatedString(item);
        }

        /// <summary>
        /// Handles the Startup event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string splitArgument = this.commaSeperatorDefault;
            if (e.Args != null && e.Args.Length > 0)
            {
                splitArgument = e.Args.FirstOrDefault();
            }

            if (splitArgument == "initMode")
            {
                this.PinJumpLists();
            }
            else
            {
                this.ExecuteCSVActions(splitArgument);
            }

            JumpList jumpList2 = JumpList.GetJumpList(App.Current);
            jumpList2.Apply();
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Adds the jump list task for the specified task name
        /// </summary>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="description">The description.</param>
        /// <param name="argument">The argument.</param>
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

        /// <summary>
        /// Executes the CSV actions i.e. reads the information from clipboard splits, concatenates based on the config 
        /// and resets copies it back to clipboard
        /// </summary>
        /// <param name="actionType">Type of the action.</param>
        private void ExecuteCSVActions(string actionType)
        {
            // ignore the leading and ending double quote characters
            string inputText = Clipboard.GetText();
            string outPutText = string.Empty;
            var allItems = actionType.Split(new string[] { ParameterDelimiter }, StringSplitOptions.None);
            int maxLines = -1;
            int.TryParse(allItems[3], out maxLines);
            outPutText = this.ConvertInputText(inputText, allItems[0], allItems[1], allItems[2], maxLines);

            Clipboard.SetText(outPutText);
        }

        /// <summary>
        /// Converts the input string into a delimited file based on the separation parameters
        /// </summary>
        /// <param name="inputText">The input text which needs to modified.</param>
        /// <param name="splitChar">The split character based on which the string will be split.</param>
        /// <param name="delimiter">The delimiter to be appended the split string.</param>
        /// <param name="encloseCharacter">The character to be enclosed when for each of the delimited field.</param>
        /// <param name="lengthPerLine">Number of fields to be present per line, for example for database; if the input field has 2000 lines and the length per line is 1000 then in each
        /// line we will have 1000 characters.</param>
        /// <returns>Returns the converted string based on the input parameters</returns>
        private string ConvertInputText(string inputText, string splitChar, string delimiter, string encloseCharacter = "'", int lengthPerLine = -1)
        {
            string[] splitArray = inputText.Split(splitChar.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string outText = string.Empty;
            if (lengthPerLine != -1)
            {
                int arrayLength = splitArray.Length;
                int skipCount = 0;
                do
                {
                    int takeCount = arrayLength - lengthPerLine > 0 ? lengthPerLine : arrayLength;

                    // conver the first n characters and append a line to it
                    outText += encloseCharacter + string.Join(
                        encloseCharacter + delimiter + encloseCharacter,
                        splitArray.Skip(skipCount).Take(takeCount)) + encloseCharacter;

                    outText += Environment.NewLine;

                    arrayLength = arrayLength - lengthPerLine;
                    skipCount += lengthPerLine;
                }
                while (arrayLength > 0);
            }
            else
            {
                outText = encloseCharacter + string.Join(encloseCharacter + delimiter + encloseCharacter, splitArray) + encloseCharacter;
            }

            return outText;
        }

        /// <summary>
        /// Called when [jump items removed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="JumpItemsRemovedEventArgs"/> instance containing the event data.</param>
        private void OnJumpItemsRemoved(object sender, JumpItemsRemovedEventArgs e)
        {
            // no code
        }

        /// <summary>
        /// Called when [jump items rejected].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="JumpItemsRejectedEventArgs"/> instance containing the event data.</param>
        private void OnJumpItemsRejected(object sender, JumpItemsRejectedEventArgs e)
        {
            // no code
        }

        /// <summary>
        /// Pins the jump lists.
        /// </summary>
        private void PinJumpLists()
        {
            // this is init section
            JumpList jumpList1 = JumpList.GetJumpList(App.Current);
            jumpList1.JumpItems.Clear();

            // get the config section from app.config file
            var csvConfigSection = ConfigurationManager.GetSection(StringSplitConfigDataSection.SectionName) as StringSplitConfigDataSection;

            if (csvConfigSection != null && csvConfigSection.CsvConfigCollection.Count > 0)
            {
                foreach (var item in csvConfigSection.CsvConfigCollection)
                {
                    CSVConfigItem configItem = item as CSVConfigItem;
                    string name = configItem.Name;

                    string fieldArgument = this.GetConcatenatedString(configItem);
                    this.AddJumpListTask(configItem.Name, string.Empty, fieldArgument);
                }
            }

            JumpList jumpList2 = JumpList.GetJumpList(App.Current);
            jumpList2.Apply();
        }

        /// <summary>
        /// Gets various fields of the config item which a custom delimiter
        /// </summary>
        /// <param name="configItem">The configuration item.</param>
        /// <returns>Returns a concatenated string</returns>
        private string GetConcatenatedString(CSVConfigItem configItem)
        {
            string outPut = @"""" +
                this.GetUnescapedCharacter(configItem.SplitChar) + ParameterDelimiter
                + this.GetUnescapedCharacter(configItem.FieldDelimiter) + ParameterDelimiter
                + this.GetUnescapedCharacter(configItem.EncloseCharacter) + ParameterDelimiter
                + this.GetUnescapedCharacter(configItem.MaxItemsPerLine.ToString()) + ParameterDelimiter + @"""";
            return outPut;
        }

        /// <summary>
        /// Gets the unescaped character. If the input begins with \ then this method attempts to get
        ///  the equivalent ASCII string else returns the original input
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Returns unescaped sequence for example if the input has value "\t" then the output will be an ASCII equivalent tab string</returns>
        private string GetUnescapedCharacter(string input)
        {
            if (input.StartsWith(@"\"))
            {
                return System.Text.RegularExpressions.Regex.Unescape(input);
            }

            return input;
        }
    }
}