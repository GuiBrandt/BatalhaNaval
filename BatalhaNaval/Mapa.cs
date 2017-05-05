using System;

namespace BatalhaNaval
{
    /// <summary>
    /// Classe para os mapas de batalha naval
    /// </summary>
    public class Mapa
    {
        public Mapa()
        {
        }

        public bool EstaCompleto()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Tabuleiro que pode ser usado num jogo
    /// </summary>
    public class Tabuleiro
    {
        /// <summary>
        /// Largura máxima do tabuleiro
        /// </summary>
        const int LarguraMaxima = 10;

        /// <summary>
        /// Altura máxima do tabuleiro
        /// </summary>
        const int AlturaMaxima = 10;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="mapa">Mapa a partir do qual criar o final</param>
        public Tabuleiro(Mapa mapa) : base()
        {
            if (!mapa.EstaCompleto())
                throw new Exception("O mapa final não pode estar incompleto");
        }
    }
}