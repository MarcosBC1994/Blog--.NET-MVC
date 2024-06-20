namespace blogteste.Models
{
    public class artigos
    {
        public int id { get; set; }
        public string titulo { get; set; }
        public string conteudo { get; set; }
        public string autor { get; set; }
        public DateTime data { get; set; }

        public string ? imagem { get; set; } // Adicionado para imagem
        public bool IsAccessible { get; set; } // Adicionado para acessibilidade
    }
}