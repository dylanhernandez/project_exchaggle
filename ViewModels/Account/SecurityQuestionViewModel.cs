using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exchaggle.ViewModels.Account
{
    public class SecurityQuestionViewModel
    {
        public SelectList GetSecurityQuestionListA
        {
            get
            {
                return new SelectList(new List<object>
                {
                    new { value = "In what city were you born?" , text = "In what city were you born?"  },
                    new { value = "What high school did you attend?" ,  text = "What high school did you attend?" },
                    new { value = "What is the name of your first school?" ,text =  "What is the name of your first school?"},
                    new { value = "What is your favorite movie?" ,  text = "What is your favorite movie?"},
                    new { value = "What is your mother's maiden name?" , text = "What is your mother's maiden name?"}
                }, "value", "text", 2);
            }
        }

        public SelectList GetSecurityQuestionListB
        {
            get
            {
                return new SelectList(new List<object>
                {
                    new { value = "What was the make of your first car?" , text = "What was the make of your first car?"  },
                    new { value = "When is your anniversary?" , text = "When is your anniversary?" },
                    new { value = "What is your favorite color?" , text = "What is your favorite color?"},
                    new { value = "What is your father's middle name?" , text = "What is your father's middle name?"},
                    new { value = "What is the name of your first grade teacher?" , text = "What is the name of your first grade teacher?"}
                }, "value", "text", 2);
            }
        }
    }
}