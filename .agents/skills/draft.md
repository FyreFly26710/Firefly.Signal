## Feature.Api.csproj
// Feature: a group of related use cases
// Resource: Use case, can have multiple related tables/Entities
// Action: usually write
// Actioned: side effect triggered by action (not necessaily has an action, api could trigger event directly)
// Mapper: DO NOT USE AUTOMAPPER, use extension methods
// Records: ALWAYS USE named arguements to create a record: new Job(Id:1, Name:"Demo"); 

- Apis/
	- (Resource)sApi.cs
	- (Resource)sApiMappers.cs // request -> command/query/event
	- FeatureService.cs 
		// FeatureService registers all applications (Mediator, IQuery, IService)
		// One per api
- Contracts/
	- Requests/
		- XxxRequest.cs
	- Responses/
		- XxxResponse.cs
		// Command/Query may produce Responses 
		// Api(Request) -> Handler(Command/Query/Event) -> Context (Entity)
		// Context (Entity) -> Handler (Response) -> Api (Response)
- Application/
	// Handler build entities without mapping
	- Commands/ 
		- (Action)Command.cs  // Mediator
		- (Action)CommandHandler.cs // inject Context, IService if needed
			// no cache read/write, invalidate via Events
	- Queries/
		- I(Resource)Queries.cs // not using Mediator
		- (Resource)Queries.cs 
			// inject Context, IService if needed, ICacheService
			// QueryHandler define cache TTL
		- (Resource)QueryModels.cs // only stores Query (param for IQueries)
	- IntergrationEventHandlers/
		- (Actioned)IntegrationEventHandler.cs // EventBus
	- DomainEventHandlers/
		- (Actioned)DomainEventHandler.cs // Mediator, eg. invalidate cache. 
	- Mappers/
		- (Resource)ResponseMappers.cs // Entity -> Response
- Infrastructure/
	- Persistence/
		- FeatureContext.cs
	- Services/
		- (External)Service/
			- I(External)Service.cs
			- (External)Service.cs
			- (External)ServiceModels.cs // hold multiple models
			- (External)ServiceMapper.cs // their model to api request/response
	- Cache/  
		- ICacheService.cs  
		- RedisCacheService.cs  
		- CacheKeys.cs //{Feature}:{Resource}:{Identifier/ParamsHash}
		- CachePolicies.cs // short 2min, med 10mins, long 6hrs

- Domain/
	- Entity/ 
		// db objects
	- Constants/ 
		// enums, consts
	- DomainEvents/ 
		// publish by Mediator
- Extensions/
	// Utils & Registration
- GlobalUsing.cs


## EventBus.csproj
- Abstractions/
	- EventBusSubcriptionInfo.cs
	- IEventBus.cs
	- IEventBusBuilder.cs
	- IIntegrationEventHandler.cs
	- IntegrationEvent.cs
- Events/
	- Feature/
		- (Actioned)IntegrationEvent.cs
	// An Event is "something that has happened in the past"
	// Add integration event contracts here for sharing. 
- Extensions
	// Utils