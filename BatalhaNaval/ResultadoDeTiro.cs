using System;

namespace BatalhaNaval
{
    /// <summary>
    /// Enumerador para resultados de tiro
    /// </summary>
    [Flags]
    public enum ResultadoDeTiro
    {
        // Resultado
        Errou   = 0x000000,
        Acertou = 0x100000,
        Afundou = 0x200000 | Acertou,

        // Tipos de barco   
        //
        // Primeiro dígito: Tamanho
        // Dígitos seguintes: Binário para ao ID do barco
        PortaAvioes = 0x50001,
        Encouracado = 0x40010,
        Cruzador    = 0x30011,
        Submarino   = 0x20100,
        Destroier   = 0x20101
    }

    /// <summary>
    /// Classe de extensão para o enumerador de resultados de tiro
    /// </summary>
    internal static class ResultadoDeTiro__Ex
    {
        /// <summary>
        /// Obtém o tamanho do navio acertado por um tiro
        /// </summary>
        public static int TamanhoDoNavio(this ResultadoDeTiro r)
        {
            return ((int)r & 0xf0000) >> 4;
        }

        /// <summary>
        /// Obtém o tipo de navio acertado por um tiro
        /// </summary>
        public static ResultadoDeTiro TipoDeNavio(this ResultadoDeTiro r)
        {
            return (ResultadoDeTiro)((int)r & 0xfffff);
        }
    }
}
