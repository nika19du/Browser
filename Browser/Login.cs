using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Browser;
namespace Browser
{
    public partial class Login : Form
    {
        public readonly BrowserContext context = new BrowserContext();
        public string URL;
        public string Title;
        public bool isClicked=true; 
        public Login()
        { 
            InitializeComponent(); 
        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.Close();
        } 
        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            { 
                var username = txtUserName.Text;
                var pass = txtPass.Text;
                User user = new User()
                {
                    UserName = username,
                    Password = pass
                };
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pass))
                {
                    MessageBox.Show("Fill in the empty form!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                } 
                var checkUser = context.Users.FirstOrDefault(x=>x.UserName==user.UserName&&x.Password==user.Password);
                if (checkUser != null)
                {
                    checkUser.IsLogIn = true;
                    context.SaveChanges();  
                    if (isClicked)
                    {
                       Form1 form1 = new Form1(checkUser, Title, URL,isClicked); 
                    }
                     
                    this.Close(); 

                }
                else if (context.Users.Where(x => x.Password != pass && x.UserName == username).Count() > 0)
                {
                    MessageBox.Show("Invalid password!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"{username} doesn't exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //register link
        private void registerLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register register = new Register();
            register.ShowDialog();
            this.Close();
        }

        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.btnLogin_Click(this,null);
            }
        }
    }
}
