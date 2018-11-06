# PROJECT-TITLE

PROJECT-DESCRIPTION

## Sumário

- [Configurações](#markdown-header-configuracoes)  
  - [Worker](#worker)  
  - [Banco de Dados](#banco-de-dados)  
  - [Logger](#logger)
- [Deploy](#deploy)  
- [Instalação](#instalação)  


## Configurações  

### Worker
A configuração do intervalo para a execução do worker pode ser configurado pelo arquivo `appsettings.json`, que está na pasta raiz do projeto, incluindo o objeto abaixo.  
```
{
    "Worker": {
        "DelayExecutionInMiliSeconds": 1000
    }
}
```
> Esse arquivo deve ser distribuído junto com a aplicação

### Banco de Dados  

A configuração com o banco de dados deve ser feita utilizando as variáveis de ambiente abaixo:  
```
DB_DATASOURCE = Texto com as configurações para conexão com o banco
DB_USERNAME = Texto com o usuário de conexão ao banco
DB_PASSWORD = Texto com a senha de conexão ao banco
DB_POOLING = [true | false] (Padrão: false)
DB_POOLING_MIN_SIZE = Valor numérico indicando o tamanho minimo do pool de conexão (Padrão: 3)
DB_POOLING_MAX_SIZE = Valor numérico indicando o tamanho máximo do pool de conexão (Padrão: 5)
```
> Quando a variável `DB_POOLING` não é definida, ou é definida como `false` as variáveis `DB_POOLING_MIN_SIZE` e `DB_POOLING_MAX_SIZE` são desconsideradas.  

### Logger  

As configurações do logger podem ser realizadas no arquivo `appsettings.json`, incluindo os objetos abaixo.  

```
{
    "Logging": {
        "Logstash": {
            "LogLevel": {
                "Default": "Information"
            }
        }
    },
    "Logstash": {
        "AppName": "PROJECT-TITLE",
        "Host": "", // Nome ou IP do servidor
        "Port": 5030, // Porta para conexão com o servidor
        "ExtraValues": { // Informações complementares que serão enviadas no log
            "author": "PROJECT-AUTHOR"
        }
    }
}  
```  
Na propridade `Logstash.ExtraValues` é possível incluir propriedades que devem ser enviadas em todos os registros de log.

## Deploy
Para realizar o deploy da aplicação basta executar o comando ['dotnet publish'](https://docs.microsoft.com/pt-br/dotnet/core/tools/dotnet-publish?tabs=netcore21) como exemplo abaixo:  
```
donet publish -c Release -o ./publish
```

## Instalação
Após o deploy, o Worker pode ser executado como um `console application` ou como um `windows service`.  

### Console Application
```
dotnet {nome_aplicacao}.dll
```

### Windows Service
```
dotnet {nome_aplicacao}.dll action:install
```  
> Esse comando tem efeito somente em ambientes windows, em outros ambientes a aplicação será executada como console. Mais informações sobre a instalação e configuração do serviço windows podem ser incontradas [aqui](https://github.com/PeterKottas/DotNetCore.WindowsService).