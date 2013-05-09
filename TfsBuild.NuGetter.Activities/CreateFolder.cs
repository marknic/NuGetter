using System.Drawing;
using System.Activities;
using System.IO;
using Microsoft.TeamFoundation.Build.Client;

// ==============================================================================================
// http://NuGetter.codeplex.com/
//
// Author: Mark S. Nichols
//
// Copyright (c) 2013 Mark Nichols
//
// This source is subject to the Microsoft Permissive License. 
// ==============================================================================================

namespace TfsBuild.NuGetter.Activities
{
    [ToolboxBitmap(typeof(CreateFolder), "Resources.nugetter.ico")]
    [BuildActivity(HostEnvironmentOption.All)]
    [BuildExtension(HostEnvironmentOption.All)]
    public sealed class CreateFolder : CodeActivity
    {
        #region Workflow Parameters
        /// <summary>
        /// Build Folder 
        /// </summary>
        [RequiredArgument]
        public InArgument<string> DropLocation { get; set; }

        /// <summary>
        /// The folder to create
        /// </summary>
        [RequiredArgument]
        public InArgument<string> FolderName { get; set; }

        public OutArgument<string> FolderCreated { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            #region Extract Workflow Argument Values

            // Build Folder
            var dropLocation = DropLocation.Get(context);

            // The folder to create
            var folderName = FolderName.Get(context);

            #endregion

            var folderCreated = DoCreateFolder(dropLocation, folderName);

            context.SetValue(FolderCreated, folderCreated);
            //FolderCreated.Set(context, folderCreated);
        }

        public string DoCreateFolder(string dropLocation, string folderName)
        {
            string folderToCreate;

            if (Path.IsPathRooted(folderName))
            {
                folderToCreate = folderName;
            }
            else
            {
                folderToCreate = Path.Combine(dropLocation, folderName);
            }

            if (!Directory.Exists(folderToCreate))
            {
                Directory.CreateDirectory(folderToCreate);
            }

            return folderToCreate;
        }
    }
}
