## 세션 목표
- 내 데이터로 질문에 답하게 해보자

## 프로젝트 시작 위치
- src/wms-start
- 실습을 위해서는 .NET 9이 설치되어 있어야 합니다.

## 내 DB 데이터로 답하게 해보자
1. 실습 프로젝트를 엽니다.
	- src/wms-start
	- aichatweb 템플릿을 이용한 프로젝트
	- WBS로 사용할 DB는 Sqlite로 EF Core를 이용하여 연결되어 있습니다.
	- Semantic Kernel Nuget 패키지가 설치되어 있습니다.
	- AI Extension을 이용한 구성이었는데 Semantic Kernel을 사용하도록 해야 합니다.

1. Programs.cs에서 웹서비스가 생성될 때 Semantic Kernel을 통해 구현되도록 코드를 변경합니다.
	1. Semantic Kernel을 사용하도록 Kernel 및 ChatClient 생성
	``` cs
	#pragma warning disable SKEXP0001
	var kernel = Kernel.CreateBuilder().
	                    AddAzureOpenAIChatCompletion("gpt-4o-mini", "https://blazormeetup.openai.azure.com", "f09bbed482e84d30999f935effe34430")
	                    .Build();

	var chatService = kernel.GetRequiredService<IChatCompletionService>();
	IChatClient chatClient = chatService.AsChatClient();
	#pragma warning restore SKEXP0001

	```

	1. 혹시 using이 필요하다면 다음 코드를 참고하세요.
	```cs
	using Microsoft.SemanticKernel;
	using Microsoft.SemanticKernel.ChatCompletion;
	```

	1. Page/Chat/Chat.razor의 SystemPrompt를 다음과 같이 바꾸어 주세요.
	```cs
   private const string SystemPrompt = @"
        당신은 창고의 재고 수량을 관리하는 역할을 가지고 있습니다.
        수량 확인이 안 되면 찾을 수 없다고 답변해주세요.";
	```

	1. plugin 생성 
		- 다음과 같이 재고 수량을 계산하는 플러그인 클래스를 만듭니다.
		```cs
		public class InventoryPlugin
		{
		    [KernelFunction("get_total_count")]
		    [Description("전체 품목의 개수를 구하는 함수")]
		    [return: Description("전체 품목의 총합계")]
		    public int getTotalCount()
		    {
		        using (var db = new FactoryContext())
		        {
		            return db.Inventories.Sum(x => x.Quantity);
		        }
		    }

		    [KernelFunction("get_count_by_item_name")]
		    [Description("품목별 재고수량을 구하는 기능")]
		    [return: Description("특정 품목의 재고수량 합계")]
		    public int getCountByItem([Description("품목이름")] string itemname)
		    {
		        using (var db = new FactoryContext())
		        {
		            return db.Inventories.Where(x => x.ItemName == itemname).Sum(x => x.Quantity);
		        }
		    }

		    [KernelFunction("get_item_list_of_warehouse")]
		    [Description("창고에 있는 품목 정보를 조회")]
		    [return: Description("품목 정보")]
		    public List<Inventory> getItemListByWhsName([Description("창고이름")] string whsname)
		    {
		        using (var db = new FactoryContext())
		        {
		            return db.Inventories.Where(x => x.WhsName == whsname).ToList();
		        }
		    }
		}
		```

		1. using이 필욯다면 아래 코드를 참고하세요.
		```cs
		using System.ComponentModel;

		using wms_start.Sqlite;

		using Microsoft.SemanticKernel;

		namespace wms_start;
		```

	1. plugin 등록
		- Programs.cs에서 kernel에 Plugin을 추가합니다.
		```cs
		kernel.ImportPluginFromType<InventoryPlugin>();
		```
		- 각 페이지에서 사용할 수 있게 주입합니다.
		```cs
		builder.Services.AddSingleton<Kernel>(kernel);
		```

		- 최종적인 코드는 다음과 같습니다.
		```cs
		#pragma warning disable SKEXP0001
		var kernel = Kernel.CreateBuilder().
		                    AddAzureOpenAIChatCompletion("gpt-4o-mini", "https://blazormeetup.openai.azure.com", "f09bbed482e84d30999f935effe34430")
		                    .Build();
		kernel.ImportPluginFromType<InventoryPlugin>();

		var chatService = kernel.GetRequiredService<IChatCompletionService>();
		IChatClient chatClient = chatService.AsChatClient();
		#pragma warning restore SKEXP0001

		builder.Services.AddChatClient(chatClient).UseFunctionInvocation().UseLogging();
		builder.Services.AddSingleton<Kernel>(kernel);
		```

	1. AI에 질의하는 부분을 수정합니다. 주입받은 Kernel을 사용할 수 있게 선언합니다. Pages/Chat/Chat.razor 페이지를 수정해 주세요.
		- @inject Kernel semanticKernel

	1. Semantic Kernel로 질의하고 메시지 목록에 추가해 주세요.
		```cs
		PromptExecutionSettings settings = new()
		{
		    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
		};
		KernelArguments arguments = new(settings);

		var response = await semanticKernel.InvokePromptAsync(userMessage.Text, arguments);
		responseText.Text = response.ToString();
		```
## 꼭 알고 넘어가야 할 것들
- AI 웹앱을 만들기 위한 템플릿이 있습니다.
