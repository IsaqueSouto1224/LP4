using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace API_paises
{
    public partial class TelaPrincipal : Form
    {
        public TelaPrincipal()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string pais = txtRespostas1.Text.Trim();

            if (string.IsNullOrEmpty(pais))
            {
                MessageBox.Show("Por favor, insira o nome de um pa�s.");
                return;
            }

            string url = $"https://restcountries.com/v3.1/name/{pais}";

            await ObterDadosPais(url);
        }

        private async void buttonRandom_Click(object sender, EventArgs e)
        {
            string url = "https://restcountries.com/v3.1/all";

            using (var client = new HttpClient())
            {
                try
                {
                    var resposta = await client.GetStringAsync(url);
                    var dados = JArray.Parse(resposta);

                    // Seleciona um pa�s aleat�rio
                    Random random = new Random();
                    int indexAleatorio = random.Next(dados.Count);
                    var paisAleatorio = dados[indexAleatorio];

                    // Extrai as informa��es do pa�s aleat�rio
                    await ExibirDadosPais(paisAleatorio);
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("Erro: N�o foi poss�vel conectar ao servidor.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private async Task ObterDadosPais(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var resposta = await client.GetStringAsync(url);
                    var dados = JArray.Parse(resposta);
                    await ExibirDadosPais(dados[0]);
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("Erro: N�o foi poss�vel conectar ao servidor.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

        private async Task ExibirDadosPais(JToken pais)
        {
            var nome = pais["name"]["common"].ToString();
            var capital = pais["capital"]?.First?.ToString() ?? "Sem capital";
            var populacao = pais["population"].ToString();
            var area = pais["area"]?.ToString() ?? "Sem �rea";
            var subregiao = pais["subregion"]?.ToString() ?? "Sem sub-regi�o";
            var regiao = pais["region"]?.ToString() ?? "Sem regi�o";
            var moeda = pais["currencies"]?.First?.First?["name"]?.ToString() ?? "Sem moeda";  
            var simboloMoeda = pais["currencies"]?.First?.First?["symbol"]?.ToString() ?? "Sem s�mbolo"; 
            var idiomas = string.Join(", ", pais["languages"]?.ToObject<JObject>().Properties().Select(x => x.Value.ToString()) ?? new string[0]);
            var continente = pais["continents"]?.First?.ToString() ?? "Sem continente";

            txtRespostas1.Text = $"{nome}";
            txtResposta2.Text = $"{capital}";
            txtResposta3.Text = $"{populacao}";
            txtResposta4.Text = $"{area} km�";
            txtResposta5.Text = $"{subregiao}";        
            txtResposta6.Text = $"{regiao}";
            txtResposta7.Text = $"{moeda}";          
            txtResposta8.Text = $"{idiomas}";
            textBox5.Text = $"{continente}";


            var bandeiraUrl = pais["flags"]["png"].ToString();
            pictureBox1.Load(bandeiraUrl);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }


        private async void buttonBuscarPorIdioma_Click(object sender, EventArgs e)
        {
            string idioma = txtIdioma.Text.Trim();

            if (string.IsNullOrEmpty(idioma))
            {
                MessageBox.Show("Por favor, insira o nome de um idioma.");
                return;
            }

            string url = "https://restcountries.com/v3.1/all";

            using (var client = new HttpClient())
            {
                try
                {
                    var resposta = await client.GetStringAsync(url);
                    var dados = JArray.Parse(resposta);

                    var paisesEncontrados = new List<string>();
                    bool encontrou = false; 

                    foreach (var pais in dados)
                    {
                        var linguas = pais["languages"];

                        if (linguas != null && linguas.HasValues)
                        {
                            foreach (var lingua in linguas)
                            {
                                if (lingua.First.ToString().Equals(idioma, StringComparison.OrdinalIgnoreCase))
                                {
                                    paisesEncontrados.Add(pais["name"]["common"].ToString());
                                    encontrou = true; 
                                    break; 
                                }
                            }
                        }
                    }

                    if (!encontrou)
                    {
                        MessageBox.Show("Nenhum pa�s encontrado que fale esse idioma.", "Resultados");
                        return;
                    }

                    string resultado = $"Pa�ses que falam \"{idioma}\":\n\n" + string.Join(", ", paisesEncontrados) + ".";

                    MessageBox.Show(resultado, "Resultados");
                }
                catch (HttpRequestException)
                {
                    MessageBox.Show("Erro: N�o foi poss�vel conectar ao servidor.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro: " + ex.Message);
                }
            }
        }

    }
}
