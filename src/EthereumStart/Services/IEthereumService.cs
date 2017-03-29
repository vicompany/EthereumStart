using System.Threading.Tasks;
using EthereumStart.Models;
using Nethereum.Contracts;

namespace EthereumStart.Services
{
    public interface IEthereumService
    {
        string AccountAddress { get; set; }
        Task<bool> SaveContractToTableStorage(EthereumContractInfo contract);
        Task<EthereumContractInfo> GetContractFromTableStorage(string name);
        Task<decimal> GetBalance(string address);
        Task<bool> ReleaseContract(string name, string abi, string byteCode, int gas);
        Task<string> TryGetContractAddress(string name);
        Task<Contract> GetContract(string name);
    }
}
