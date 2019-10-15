using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace OutlookInvitePrevention
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Application.ItemSend += Application_ItemSend;
        }

        private void Application_ItemSend(object Item, ref bool Cancel)
        {
            Outlook.MailItem mail = Item as Outlook.MailItem;
            if (mail == null) return;

            string content = $"{mail.Subject} {mail.Body}";
            bool isAnInviteMail = Regex.IsMatch(content, @"invit[a-zÀ-ÿ0-9]*", RegexOptions.IgnoreCase);
            if (!isAnInviteMail) return;

            bool authorizedToInvite = FaceChecker.GuessIfIAmInFrontOfMyLaptop();
            if (!authorizedToInvite)
            {
                Cancel = true;
                LockWorkStation();
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Nota: Outlook ya no genera este evento. Si tiene código que 
            //    se debe ejecutar cuando Outlook se apaga, consulte https://go.microsoft.com/fwlink/?LinkId=506785
        }

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        #region Código generado por VSTO

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
