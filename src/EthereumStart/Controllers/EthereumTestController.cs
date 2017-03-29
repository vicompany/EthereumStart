using EthereumStart.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EthereumStart.Controllers
{
    [Route("api/[controller]")]
    public class EthereumTestController : Controller
    {
        private IEthereumService service;
        private const string abi = @"[{""constant"":false,""inputs"":[{""name"":""add"",""type"":""uint256""}],""name"":""addCoins"",""outputs"":[{""name"":""b"",""type"":""uint256""}],""payable"":false,""type"":""function""},{""constant"":false,""inputs"":[{""name"":""add"",""type"":""uint256""}],""name"":""subtractCoins"",""outputs"":[{""name"":""b"",""type"":""uint256""}],""payable"":false,""type"":""function""},{""constant"":true,""inputs"":[],""name"":""balance"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""type"":""function""},{""inputs"":[{""name"":""initial"",""type"":""uint256""}],""payable"":false,""type"":""constructor""}]";
        private const string byteCode = "0x6060604052341561000c57fe5b604051602080610185833981016040528080519060200190919050505b806000819055505b505b610143806100426000396000f30060606040526000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680630173e3f41461005157806349fb396614610085578063b69ef8a8146100b9575bfe5b341561005957fe5b61006f60048080359060200190919050506100df565b6040518082815260200191505060405180910390f35b341561008d57fe5b6100a360048080359060200190919050506100f8565b6040518082815260200191505060405180910390f35b34156100c157fe5b6100c9610111565b6040518082815260200191505060405180910390f35b600081600054019050806000819055508090505b919050565b600081600054039050806000819055508090505b919050565b600054815600a165627a7a723058200085d6d7778b3c30ba2e3bf4af4c4811451f7367109c1a9b44916d876cb67c5c0029";
        private const int gas = 4700000;

        public EthereumTestController(IEthereumService ethereumService)
        {
            service = ethereumService;
        }

        [HttpGet]
        [Route("getBalance/{walletAddress}")]
        public async Task<decimal> GetBalance([FromRoute]string walletAddress)
        {
            return await service.GetBalance(walletAddress);
        }
        [HttpGet]
        [Route("releaseContract/{name}")]
        public async Task<bool> ReleaseContract([FromRoute] string name)
        {
            return await service.ReleaseContract(name, abi, byteCode, gas);
        }

        [HttpGet]
        [Route("checkContract/{name}")]
        public async Task<bool> CheckContract([FromRoute] string name)
        {
            return await service.TryGetContractAddress(name) != null;
        }

        [HttpGet]
        [Route("exeContract/{name}/{contractMethod}/{value}")]
        public async Task<string> ExecuteContract([FromRoute] string name, [FromRoute] string contractMethod, [FromRoute] int value)
        {
            string contractAddress = await service.TryGetContractAddress(name);
            var contract = await service.GetContract(name);
            if (contract == null) throw new System.Exception("Contact not present in storage");
            var method = contract.GetFunction(contractMethod);         
            try
            {
                // var result = await method.CallAsync<int>(value);
                var result = await method.SendTransactionAsync(service.AccountAddress, value);
                return result.ToString();
            }
            catch (Exception ex)
            {
                return "error";
            }
        }

        [HttpGet]
        [Route("checkValue/{name}/{functionName}")]
        public async Task<int> CheckValue([FromRoute] string name, [FromRoute] string functionName)
        {
            string contractAddress = await service.TryGetContractAddress(name);
            var contract = await service.GetContract(name);
            if (contract == null) throw new System.Exception("Contact not present in storage");
            var function = contract.GetFunction(functionName);

            var result = await function.CallAsync<int>();

            return result;
        }
    }
}
