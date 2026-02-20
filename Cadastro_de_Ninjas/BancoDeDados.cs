using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Cadastro_de_Ninjas
{
    public static class BancoDeDados
    {
        // Aqui fica a rota para o seu MySQL. Se o seu tiver senha, adicione Pwd=suasenha;
        const string connectionString = "Server=localhost;Database=naruto_db;Uid=root;";

        public static bool InserirNinja(Ninja ninja)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlTransaction transaction = null;
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    // 1. Inserir primeiro os Dados de Combate (Habilidades)
                    string sqlHabilidades = @"
                        INSERT INTO DadosNinja (ElementoChakra, JutsusPrincipais, Altura, Peso, NivelDePoder)
                        VALUES (@Elemento, @Jutsus, @Altura, @Peso, @Poder);
                        SELECT LAST_INSERT_ID();";

                    int dadosNinjaId;

                    using (MySqlCommand cmdHab = new MySqlCommand(sqlHabilidades, connection, transaction))
                    {
                        cmdHab.Parameters.AddWithValue("@Elemento", ninja.Habilidades.ElementoChakra);
                        cmdHab.Parameters.AddWithValue("@Jutsus", ninja.Habilidades.JutsusPrincipais);
                        cmdHab.Parameters.AddWithValue("@Altura", ninja.Habilidades.Altura);
                        cmdHab.Parameters.AddWithValue("@Peso", ninja.Habilidades.Peso);
                        cmdHab.Parameters.AddWithValue("@Poder", ninja.Habilidades.NivelDePoder);

                        // Pega o ID que o banco gerou para esses dados
                        dadosNinjaId = Convert.ToInt32(cmdHab.ExecuteScalar());
                    }

                    // 2. Inserir o Ninja apontando para os Dados de Combate dele
                    string sqlNinja = @"
                        INSERT INTO Ninja (Nome, Idade, DataNascimento, Genero, VilaOrigem, RankNinja, DadosNinjaId)
                        VALUES (@Nome, @Idade, @DataNascimento, @Genero, @Vila, @Rank, @DadosId);";

                    using (MySqlCommand cmdNinja = new MySqlCommand(sqlNinja, connection, transaction))
                    {
                        cmdNinja.Parameters.AddWithValue("@Nome", ninja.Nome);
                        cmdNinja.Parameters.AddWithValue("@Idade", ninja.Idade);
                        cmdNinja.Parameters.AddWithValue("@DataNascimento", ninja.DataNascimento);
                        cmdNinja.Parameters.AddWithValue("@Genero", ninja.Genero);
                        cmdNinja.Parameters.AddWithValue("@Vila", ninja.VilaOrigem);
                        cmdNinja.Parameters.AddWithValue("@Rank", ninja.RankNinja);
                        cmdNinja.Parameters.AddWithValue("@DadosId", dadosNinjaId);

                        cmdNinja.ExecuteNonQuery();
                    }

                    // Confirma que deu tudo certo e salva
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar no banco de dados: " + ex.Message, "Erro MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (transaction != null)
                        transaction.Rollback(); // Se der erro, desfaz tudo
                    return false;
                }
            }
        }

        public static List<Ninja> BuscaNinjas()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    List<Ninja> ninjas = new List<Ninja>();
                    connection.Open();

                    // Busca juntando as duas tabelas
                    string sqlQuery = @"SELECT d.*, n.* FROM Ninja n INNER JOIN DadosNinja d ON d.Id = n.DadosNinjaId;";

                    using (MySqlCommand cmd = new MySqlCommand(sqlQuery, connection))
                    {
                        MySqlDataReader resultado = cmd.ExecuteReader();

                        while (resultado.Read())
                        {
                            Ninja ninja = new Ninja();

                            // Mapeando DadosNinja
                            ninja.Habilidades.ElementoChakra = resultado["ElementoChakra"].ToString();
                            ninja.Habilidades.JutsusPrincipais = resultado["JutsusPrincipais"].ToString();
                            ninja.Habilidades.Altura = Convert.ToDouble(resultado["Altura"]);
                            ninja.Habilidades.Peso = Convert.ToDouble(resultado["Peso"]);
                            ninja.Habilidades.NivelDePoder = Convert.ToDouble(resultado["NivelDePoder"]);

                            // Mapeando Ninja
                            ninja.Nome = resultado["Nome"].ToString();
                            ninja.Idade = Convert.ToInt32(resultado["Idade"]);
                            ninja.DataNascimento = Convert.ToDateTime(resultado["DataNascimento"]);
                            ninja.Genero = resultado["Genero"].ToString();
                            ninja.VilaOrigem = resultado["VilaOrigem"].ToString();
                            ninja.RankNinja = resultado["RankNinja"].ToString();

                            ninjas.Add(ninja);
                        }
                    }
                    return ninjas;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao buscar no banco: " + ex.Message, "Erro MySQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<Ninja>();
                }
            }
        }
    }
}