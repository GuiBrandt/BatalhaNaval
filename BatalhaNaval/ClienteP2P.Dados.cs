using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Timers;

namespace BatalhaNaval
{
    /// <summary>
    /// Cliente Peer-to-Peer para a batalha naval
    /// </summary>
    public sealed partial class ClienteP2P
    {
        /// <summary>
        /// Delegado para a função de controle de eventos de tiro recebido
        /// </summary>
        /// <param name="x">Posição X do tiro recebido</param>
        /// <param name="y">Posição Y do tiro recebido</param>
        public delegate void EventoDeTiroRecebido(int x, int y);

        /// <summary>
        /// Delegado para função de controle de evento de quando deve-se dar um tiro
        /// </summary>
        /// <returns>Um objeto do tipo tiro, com uma coordenada X e Y</returns>
        public delegate Tiro EventoDeDarTiro();

        /// <summary>
        /// Evento de tiro
        /// </summary>
        public event EventoDeDarTiro Atirar;

        /// <summary>
        /// Evento de tiro recebido
        /// </summary>
        public event EventoDeTiroRecebido TiroRecebido;
        
        /// <summary>
        /// Mapa usado pelo cliente
        /// </summary>
        public Tabuleiro Tabuleiro { get; private set; }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="tabuleiro">Tabuleiro com o qual se quer jogar</param>
        /// <exception cref="Exception">Se o tabuleiro estiver incompleto</exception>
        public ClienteP2P(Tabuleiro tabuleiro) : this()
        {
            if (!tabuleiro.EstaCompleto())
                throw new Exception("Tabuleiro incompleto");

            Tabuleiro = tabuleiro;
            ClienteConectado += OnClienteConectado;
        }

        /// <summary>
        /// Evento de sucesso de conexão com cliente
        /// </summary>
        /// <param name="addr">Endereço do cliente</param>
        private bool OnClienteConectado(IPAddress addr)
        {
            
            return true;
        }
    }
}
