using System.Activities;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Build.Workflow.Activities;

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
    internal static class BuildContextExtensions
    {
        /// <summary>
        /// Extension method to help writing messages to the build context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <param name="messageImportance"></param>
        public static void WriteBuildMessage(this CodeActivityContext context, string message, BuildMessageImportance messageImportance)
        {
            if (context != null)
            {
                context.TrackBuildMessage(message, messageImportance);
                                  //{ 
                                  //    Value = new BuildMessage
                                  //                {
                                  //                    Importance = messageImportance,

                                  //                    Message = message,
                                  //                },
                                  //});
            }
        }
    }
}
