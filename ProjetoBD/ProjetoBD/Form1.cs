using System; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Fernet;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualBasic.ApplicationServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Reflection.Metadata;

namespace ProjetoBD
{
    public partial class Form1 : Form
    {
        bool needsCript;
        byte[] eggs;
        string dbdbd;
        string receiver;
        string send;
        public Form1()
        {
            InitializeComponent();

            //define elementos que irao ou nn aparecer
            Image imagetrue = this.BackgroundImage;
            textBox3.Visible = false; label8.Visible = false;
            label9.Visible = false; textBox5.Visible = false;
            textBox3.Enabled = false; textBox2.Visible = false;
            label7.Visible = false; textBox4.Visible = false;
            button1.Visible = false; linkLabel1.Visible = false;
            label4.Visible = false; label5.Visible = false;
            textBox1.Visible = false; button3.Visible = false;
            button2.Visible = false;

            //conecta com o banco do Marco, user:senha@cluster.mongodb.net/?baboseirapcorrigirerro
            const string connectionUri = "mongodb+srv://letsmakeapizzapie:cHqBOlKbeiTeVX0n@cluster0.2gkb6.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0";
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);

            try
            {
                //testa conexão
                var result = client.GetDatabase("asdas").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
                label3.Text = "event: connected to DB";
                label3.ForeColor = Color.Blue;
            }
            catch (Exception ex)
            {
                //se não conectar, escreve no console o erro e define evento erro
                Console.WriteLine(ex);
                label3.Text = "event: error";
                label3.ForeColor = Color.Red;
            }


        }

        // TELA LOGIN
        private void label1_Click(object sender, EventArgs e)
        {
            //define oque é e oque não deve ser visivel na tela de login
            label2.Visible = false;
            label4.Visible = true;
            label7.Visible = false;
            label5.Visible = true;
            linkLabel1.Visible = true;
            button1.Visible = true;
            this.BackgroundImage = null;
            textBox3.Visible = true;
            textBox2.Visible = true;
            textBox3.Enabled = true;
            label3.Text = "event: attempt to log";
            label3.ForeColor = Color.LightGray;
        }

        // BOTÃO VOLTAR DA TELA LOGIN
        private void label6_Click(object sender, EventArgs e)
        {
            //define também oque deve e nn deve ser visto ao voltar para a tela de login
            label3.Text = "event: attempt to log";
            label3.ForeColor = Color.LightGray;
            Image imagetrue = this.BackgroundImage;
            textBox3.Visible = false; label8.Visible = false;
            label9.Visible = false; textBox5.Visible = false;
            textBox3.Enabled = false; textBox2.Visible = false;
            label7.Visible = false; textBox4.Visible = false;
            button1.Visible = false; linkLabel1.Visible = false;
            label4.Visible = false; label5.Visible = false;
            textBox1.Visible = false; button3.Visible = false;
            button2.Visible = false;

            label2.Visible = false;
            label4.Visible = true;
            label7.Visible = false;
            label5.Visible = true;
            linkLabel1.Visible = true;
            button1.Visible = true;
            this.BackgroundImage = null;
            textBox3.Visible = true;
            textBox2.Visible = true;
            textBox3.Enabled = true;
            label1.Visible = true;
        }

        // BOTÃO PARA SAIR
        private void label2_Click(object sender, EventArgs e)
        {
            Close();

        }



        private void button1_Click(object sender, EventArgs e)
        {
            //pega oque foi escrito na parte de usuario e senha e define como dbUser e dbPass
            //também define send e receiver (variaveis globais) como o user de quem vai receber e quem vai mandar as mensagens (cliente e o alvo desejado)
            //dbCheckUser é o usuario a ser contatado.
            string dbUser = textBox3.Text;
            send = dbUser;
            string dbPass = textBox2.Text;
            string dbCheckUser = textBox4.Text;
            dbdbd = dbCheckUser;
            receiver = dbCheckUser;
            //reconecta com cliente (atoa)
            //database entra no banco de dados asdas
            var client = new MongoClient(MongoClientSettings.FromConnectionString("mongodb+srv://letsmakeapizzapie:cHqBOlKbeiTeVX0n@cluster0.2gkb6.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0"));
            var database = client.GetDatabase("asdas");
            //abre a coleção asdasd(logins)
            var collection = database.GetCollection<BsonDocument>("asdasd");
            //filtra os usuários e senhas para checar se está de acordo com as informações no banco de dados de logins de todos usuários
            //por segurança ele não puxa nenhum valor, apenas checa se pode ser encontrado e então diz se for nulo ou não, ao invés de imprimir para o usuário e comprometer os dados de outros users
            var filter = Builders<BsonDocument>.Filter.Eq("user", dbUser) &
                         Builders<BsonDocument>.Filter.Eq("pass", dbPass);
            //procura se a conta foi achada entre a coleção usando os filtros.
            var account = collection.Find(filter).FirstOrDefault();

            //colleção de criptografias (checagem se existe ou nn historico de conversa+key para [des]criptografar)
            var criptColl = database.GetCollection<BsonDocument>("criptographyas");
            var filter2 = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("pas1", dbUser),
                    Builders<BsonDocument>.Filter.Eq("pas2", dbUser)
                ),
                Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("pas1", dbCheckUser),
                    Builders<BsonDocument>.Filter.Eq("pas2", dbCheckUser)
                )
            );
            //checa com a filtragem 2 (se existe o usuário loggado e desejado no espaço pas1 e pas2, independente da ordem, desde que ambos sejam verdades, essa conexão/historico existe.)
            //se existir, recebe resultadlo
            var resultadlo = criptColl.Find(filter2).ToList();



            if (textBox2.Visible == false)
            {
                //filtra os usuários de acordo com usuário desejado á iniciar a conversa após passar da tela de login
                var filtered = Builders<BsonDocument>.Filter.Eq("user", dbCheckUser);
                //se achar o usuário que você deseja conversar...
                var checking = collection.Find(filtered).FirstOrDefault();
                if (checking != null && checking != account)
                {
                    //se o usuário que você deseja conversar exista e não seja você...
                    MessageBox.Show("Connected!");
                    label8.Visible = false; textBox4.Visible = false;
                    label9.Visible = false; button1.Visible = false;
                    textBox5.Visible = true; textBox1.Visible = true;
                    button3.Visible = true; button2.Visible = true;
                    label1.Visible = false;
                    //ativa e desativa de acordo com a tela desejada (você sai da tela de digitar pessoa á conversar e passa a conversar com a pessoa.
                    label3.Text = "event: connected to convo";
                    label3.ForeColor = Color.Green;

                    if (resultadlo != null && resultadlo.Count > 0)
                    {
                        //se você já conversou com a pessoa antes
                        MessageBox.Show("Bem-Vindo de volta!");
                        needsCript = false;
                    }
                    else
                    {
                        //se você nunca conversou com ela
                        MessageBox.Show("Iniciando nova conversa...");
                        needsCript = true;
                    }
                    // ambos os casos são descobertos caso haja histórico de conversa usando a checagem da tabela de criptografia, ou seja, se há uma chave para aqueles dois usuários, eles já conversaram antes.


                    label2.Visible = true;
                    //botão de voltar ao login aparece
                }
                else
                {
                    //caso nn exista o usuário que você está tentando conversar com
                    MessageBox.Show("Invalid username.");
                    label3.Text = "event: unable to connect";
                    label3.ForeColor = Color.Red;
                }
            }
            else
            {
                var collection2 = database.GetCollection<BsonDocument>("logs");

                if (account != null)
                {
                    MessageBox.Show("Login successful!");
                    //DESATIVA ITENS ANTERIORES
                    label2.Visible = false; label6.Visible = true;
                    label4.Visible = false; label7.Visible = false;
                    label5.Visible = false; linkLabel1.Visible = false;
                    textBox3.Visible = false;
                    textBox2.Visible = false; textBox3.Enabled = false;
                    label1.Visible = false;

                    //ATIVA NOVOS ITENS
                    label8.Visible = true; textBox4.Visible = true;
                    label9.Visible = true;
                    DateTime date = DateTime.Now;

                    //todas vezes que aparecer esse Convert.ToBase64String ele cria uma string aleatoria em base64.
                    var log = new BsonDocument
                    {
                        {"_id", Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(1, 21)},
                        {"user", send ?? "" },
                        {"moment", date },
                        {"action", "logged in" }
                    };
                    //manda esse log caso você faça login com a conta e a data/momento

                    try
                    {
                        collection2.InsertOneAsync(log);
                        //insere na coleção de logs o log
                        label3.Text = "event: Logged In";
                        label3.ForeColor = Color.Blue;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Houve um erro ao tentar se conectar ao log: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                    //se o user for invalido ou a senha nn esteja correta.
                    DateTime date = DateTime.Now;

                    var log = new BsonDocument
                    {
                        {"_id", Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(1, 21)},
                        {"user", send ?? "" },
                        {"moment", date },
                        {"action", "failed to login" }
                    };
                    //sempre que falhar em fzr o login, manda essa pedrada q ~~nn~~ agora tá funcionando

                    try
                    {
                        collection2.InsertOneAsync(log);
                        //insere na coleção de logs o log
                        label3.Text = "event: Failure to log in";
                        label3.ForeColor = Color.Red;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Houve um erro ao tentar se conectar ao log: {ex.Message}");
                    }

                }
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string dbdb = dbdbd;
            var client = new MongoClient(MongoClientSettings.FromConnectionString("mongodb+srv://letsmakeapizzapie:cHqBOlKbeiTeVX0n@cluster0.2gkb6.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0"));
            var database = client.GetDatabase("asdas");
            var collection = database.GetCollection<BsonDocument>("messages");
            var filter = Builders<BsonDocument>.Filter.Eq("sender", receiver);
            var messages = collection.Find(filter).ToList();
            textBox5.Clear();

            var criptic = database.GetCollection<BsonDocument>("criptographyas");

            var filtr = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("pas1", send),
                    Builders<BsonDocument>.Filter.Eq("pas2", receiver)
                ),
                Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("pas1", receiver),
                    Builders<BsonDocument>.Filter.Eq("pas2", send)
                )
            );

            foreach (var message in messages)
            {
                var senderr = message["sender"].AsString;
                var content = message["message"].AsString;
                var timestamp = message["timestamp"];

                var results = criptic.Find(filtr).FirstOrDefault();

                if (results != null)
                {
                    string key = results["key"].AsString;
                    MessageBox.Show("Chave encontrada! \r\n é recomendado escrever uma \r\n mensagem antes de carregar \r\n as mensagens anteriores.");
                    byte[] byteArray = Convert.FromBase64String(key);
                    var decryptedBytes = SimpleFernet.Decrypt(byteArray, content, out var decryptedTimestamp);
                    string decryptedContent = System.Text.Encoding.UTF8.GetString(decryptedBytes);
                    textBox5.AppendText($"{timestamp} - {senderr}: {decryptedContent} \r\n");
                }
                else
                {
                    textBox5.AppendText("Chave não encontrada para a mensagem.\r\n");
                }

                label3.Text = "event: RECEIVED MESSAGE!";
                label3.ForeColor = Color.Blue;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string dbdb = dbdbd;
            var client = new MongoClient(MongoClientSettings.FromConnectionString("mongodb+srv://letsmakeapizzapie:cHqBOlKbeiTeVX0n@cluster0.2gkb6.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0"));
            var database = client.GetDatabase("asdas");
            var collection = database.GetCollection<BsonDocument>("messages");
            DateTime date = DateTime.Now;
            var criptic = database.GetCollection<BsonDocument>("criptographyas");
            var filtr = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("pas1", send),
                    Builders<BsonDocument>.Filter.Eq("pas2", receiver)
                ),
                Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("pas1", receiver),
                    Builders<BsonDocument>.Filter.Eq("pas2", send)
                )
            );
            var projection = Builders<BsonDocument>.Projection.Include("key");
            var results = criptic.Find(filtr).Project(projection).FirstOrDefault();
            string standardBase64Key;
            string key;
            if (results != null && results.Contains("key"))
            {
                key = results["key"].AsString;
                standardBase64Key = key.Replace('-', '+').Replace('_', '/');
            }
            else
            {
                key = SimpleFernet.GenerateKey();
                standardBase64Key = key.Replace('-', '+').Replace('_', '/');
            }

            byte[] trues = Convert.FromBase64String(standardBase64Key);
            string message = textBox1.Text;
            var messageBytes = message.ToBase64String();
            var token = SimpleFernet.Encrypt(trues, messageBytes.UrlSafe64Decode());

            if (needsCript == true)
            {
                var messagecript = new BsonDocument
                {
                    {"_id", Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(1, 21) },
                    {"key", standardBase64Key ?? "" },
                    {"pas1", send ?? "" },
                    {"pas2", receiver ?? "" }
                };


                var newMessage = new BsonDocument
                {
                    {"_id", Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(1, 21)},
                    {"sender", send ?? "" },
                    {"receiver", receiver ?? "" },
                    {"message", token },
                    {"timestamp", date }
                };

                try
                {
                    collection.InsertOneAsync(newMessage);
                    criptic.InsertOneAsync(messagecript);
                    MessageBox.Show("Sent!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"There was an Error sending your message: {ex.Message}");
                }

                needsCript = false;
            }
            else
            {
                var newMessage = new BsonDocument
                {
                    {"_id", Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(1, 22)},
                    {"sender", send ?? "" },
                    {"receiver", receiver ?? "" },
                    {"message", token },
                    {"timestamp", date }
                };

                try
                {
                    collection.InsertOneAsync(newMessage);
                    MessageBox.Show("Sent!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"There was an Error sending your message: {ex.Message}");
                }
            }

            label3.Text = "event: MESSAGE SENT!";
            label3.ForeColor = Color.Blue;
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            label7.Visible = true;
            label7.Text = "NN TEM";
            label3.Text = "mals ai bruno, nn tem";
            label3.ForeColor = Color.Red;
        }
    }
}
