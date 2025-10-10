// namespace reflete a localização do arquivo na pasta Service
namespace savemoney.Service;

/*  
    interface define que isto é um contrato não
    uma classe que faz algo a letra I no inicio, é
    uma convenção mt forte no C# para identificar
    rapidamente que isso é uma interface 
*/
public interface IConteudoService
{
    // esta é a assinatura do metodo que TODOS os serviços de conteudo serão obrigados a ter.
    /* Task<string>: diz que o metodo é assincrono e que quando terminar,
     ele vai devolver um (return) uam string (o nosso JSON)
    */
    // BuscarConteudoAsync: é o nome do metodo o suffixo 'async' é outra convenção para metodos assincronos
    // note que não há chaves{} nem codigo aqui, a interface só define O QUE, não COMO.
    Task<string> BuscarConteudoAsync();
}
/*

### Seus Próximos Passos (O Desafio de Estudo)

Agora que você tem o "contrato" (`IConteudoService.cs`), o seu próximo passo (**Passo 2** do nosso plano) é criar os "trabalhadores" que assinam este contrato.

1.  **Ação:** Crie a sua classe `NoticiasService.cs`.
2.  **Abstração:** Faça esta classe "assinar" o contrato. A sintaxe para isso é:
    ```csharp
    public class NoticiasService : IConteudoService 
    {
        // ...
    }
    
*/