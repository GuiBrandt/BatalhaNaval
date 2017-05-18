using System;

namespace BatalhaNaval
{
    /// <summary>
    /// Enumerador para resultados de tiro
    /// </summary>
    [Flags]
    public enum ResultadoDeTiro : uint
    {
        // Resultado
        Errou   = 0x00000000,
        Acertou = 0x10000000,
        Afundou = 0x20000000 | Acertou,
    }

    /// <summary>
    /// Enumerador para os tipos de navio
    /// 
    /// É muita magia, não mexa
    /// </summary>
    public enum Navio : uint
    {
        PortaAvioes = 0x150000,
        Encouracado = 0x240000,
        Cruzador    = 0x330000,
        Submarino   = 0x220000,
        Destroier   = 0x220001
    }

    /// <summary>
    /// Classe de extensão para o enumerador de tipos de navio
    /// </summary>
    public static class Navio_Ex
    {
        /// <summary>
        /// Obtém o tamanho do navio acertado por um tiro
        /// </summary>
        public static int Tamanho(this Navio nav)
        {
            return (int)(((uint)nav & 0x0f0000) / 0x10000);
        }

        /// <summary>
        /// Obtém o limite de um tipo de navio no mapa
        /// </summary>
        public static int Limite(this Navio nav)
        {
            return (int)((uint)nav / 0x100000);
        }
    }

    /// <summary>
    /// Classe de extensão para o enumerador de resultados de tiro
    /// </summary>
    public static class ResultadoDeTiro_Ex
    {
        /// <summary>
        /// Obtém o tipo de navio acertado por um tiro
        /// </summary>
        public static Navio TipoDeNavio(this ResultadoDeTiro r)
        {
            return (Navio)((int)r & 0xfffffff);
        }
    }
}
