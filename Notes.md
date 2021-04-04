# Blazor WebAssembly


These notes are taken following the tutorial found in:
https://code-maze.com/blazor-components/

## COMPONENTS

* Are the key pieces in this technology
* They are composed by HTML elements and code
* Of course HTML elements need to be configured to present some state to the UI.  
* This visual/content configuration has to be dynamic
* Dynamic behaviour can be achieved by indicating within the elements some variables that can be configured and injected from the container that will use the component.
* Just declare that the content depends on a `@parameter`. And then in the `@code` section specify a c# property that will yield the required content. 
Those c# properties has to be decorated with the `[Parameter]` decorator
* When declared inside a component, it is just necessary to set the value of the parameter as if it was just another html `attribute` of the component.
* Even when it is good for simple pieces of content, when trying to pass more complex content through a parameter it could be hard to read/mantain. For this purpose we can use the `RenderFragment` parameter that can represent plain text, html, or whole other components
* **`RenderFragment`**: By default, it takes the `ChildContent` as the name of the parameter. If you specify another name you have to indicate the custom name also in the container. If not, you won't need to include it.
* **`Cascading Parameters`**: Sometimes, it is desired to have some parameters to be set automatically without declaring them explicitly in each component inside the container.
	(Useful for example when a given parameter is required to be shared among various components inside a container)
	To achieve this, the attribute is declared as a `[CascadigParameter]` both in the component and in the container.
	The container declares the attribute as a field with a given value:  
  
	`private readonly string _color = "#0505b5";`  
  
	While the component declares it as a normal property with the decorator `[CascadigParameter]` the name of the property is irrelevant, 
	since Blazor just check for the type of the property and there is only one property to pass in cascade.
	So if we need to pass multiple values, we create a type with all those values.
	If desired, the cascading parameter can also be referenced by name adding some sintaxis sugar:  
  
	`<CascadingValue Name="HeadingColor" Value="@_color">`  
  
	in the container, and then  
  
	`[CascadingParameter(Name = "HeadingColor")]  `
  
	in the component

* Elements style: Can be bound to a `Dictionary<string, object>` that will contain the key/values for all the properties that the elements could have
	if the `(CaptureUnmatchedValues=true)` modificator for the `[Parameter]` decorator is set:  

	`[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object> AdditionalAttributes { get; set; }`  
				
	Then, the container (this is: the component caller), could specify those attributes from the component declaration itself:<br>

	`<Home Title="Welcome to the BlazorProducts.Client application." 
	src="/assets/products.png" alt="products image for the Home component"></Home>`  

	And those attributes would be injected wherever they apply in a normal HTML.  
	
	The container could also determine a property of type `Dictionary<string, object>` that could have the additional attributes listed inside so the code could be more readable:<br>

		<Home @attributes="AdditionalAttributes"></Home>  
		@code{
			public Dictionary<string, object> AdditionalAttributes { get; set; }
				 = new Dictionary<string, object>{
					{ "src", "/assets/products.png" },
					{ "alt", "products image for the Home component" }
			};
		}

	In this case, the container creates the dictionary that the component expects as a parameter. And the component then distributes the key/values within its HTML elements<br><br><br>

-----------------------------------

## Partial Classes

Creating a file in the same folder as a `.razor` component with the same name and the extension `.razor.cs` will create an association between the component and its code behind, but the `partial` keyword must be added manually
	

## Blazor Lifecycle	
* `StateHasChanged`: One if the most useful event used for example to order the hiding of a modal window after the user closes it. Nevertheless, other events are used esoecially when downloading data from a service. For example the `OnInitializedAsync` that is similar to the old `OnLoad` form the `WebForms` world.

## Routing & Navigation
It is enabled by injecting the service NavigationManager in the code of the component:
	
	public NavigationManager NavigationManager { get; set; }

        public void NavigateToHome()
        {
            NavigationManager.NavigateTo("/");
        }
    }

We have other utilities in the navigator like getting the current url: `NavigationManager.Uri`

## CSS Stylesheets:
Can be created anywhere. Even inside a folder of pages to better organizations using CSS Isolation. Once created, they must referenced from the INDEX.HTML <==== the HTML!!! And the cache of the site must be refreshed in order to get the styling visually updating when rendering the application.

## HTTP CLIENT
It must be registered in the ` Main`  method of the `Program class` (this is done by default).<br>
A repository pattern for http data is recommended to be implemented using the `httpclient`  service.<br>
This repository is registered also in the main method of the `program class`.
The repository is then `[Inject]ed` in each component that needs it.<br>
Within the component code, we call the data in the equivalent event to the old `OnLoad` of the `WebForms`: `OnInitializedAsync`.<br>
All of this is configured in a razor page that will contain this razor component which will have the logic and design to present the data. This component will require a `[Parameter]` field containing the data the container page sends to it.

## PAGINATION
A `class` for configuring parameters for querying a type of object is recommended to describe:<br>
* `maxPageSize`
* `currentPageNumber`
* `pageSize`<br><br>

This class should be part of the shared entities. A metadata class is also recommended to determine info about the current page we are working with. For example:
* `CurrentPage`
* `TotalPages`
* `PageSize`
* `TotalCount`
* `HasPrevious`
* `HasNext`<br><br>

This class should be used for the client side pagination. Then the backend should have a `pagedList` of entities with metadata, the paged list representing each page (with metadata and data) and of course an operation to return a paged list after querying the repository with the parameters passed. There is an important fact in the controller returning the page and it is adding a header to the response, that will contain the metadata of the page. That header is called `"X-Pagination"` (this could be simply included in the response as another field, but I guess `webassebmly` could make some auto operations based on this convention. This header must be added to the `cors policy`).

*Note:* We can create query strings easy with a `dictionary<string, string>` using `QueryHelpers.AddQueryString` from `Microsoft.AspNetCore.WebUtilities`.

### Pagination in the Client
The pagination in the client requires a component that will serve as the index of links to show. This component is basically a list of links displayed horizontally with a next and previous buttons.<br>
This list of links must be presented using some parameters. Both the links list and parameters should make part of the `codebehind` of the component. It also has to contain an `eventcallback` and a method to create the list as well as an event to react whenever a link is clicked by the user.<br>
This component will be embedded in a container where some initialization parameters are specified. Through the eventcallback, the pagination component tells the container what next page was asked and then then the container calls the service to bring that content. That is done setting the `productparameters` and with the response from the service, the fresh data is shown to the user.

## SEARCHING
It has the same basis of pagination from a component/container point of view but ships interesting additions such as the `@bind-value attribute`: This provides two-way binding with the property defined in the component. By default the value of the property is set when the user leaves the control. But since it is needed that the assignation be made exactly when a new value is written in the control, we use the `@bind-value:event` attribute set it to `oninput` value (this will reflect a "real-time behavior").

Additionally we have here a lambda expression that calls the callback so whenever a key is pressed, the value of the control is passed to the container so it can execute the service call with the current search term.<br>

In the backend, the search operation uses the `GetProducts`, just with one additional parameter: the `searchTerm`. So no additional methods are required here.

### Optimizing server use on search
Since the previous method sends a request to the server each time a user press a key and this is not optimal at all, we need to include a timer and whenever a key is pressed the timer is reset. And if no key is pressed anymore, it is time to send the request as the user now is ready to receive results. A prudential time for this wait is 500ms.

### Extension Methods as Managers for POCO
This samples have demonstrated that it is a very good strategy to enrich POCOs to add functionalities such as sorting or searching

## SORTING
To sort, we need to add another string parameter indicating the sort we want. In the backend it is recommended to install `System.Linq.Dynamic.Core` as it is very useful to make dynamic queries over our entities. It is recommended to read: https://code-maze.com/sorting-aspnet-core-webapi/ To learn more.<br>
<br>
The logic for sorting is also added to the backend repository class by using `extension methods`. And this time, even `Reflection` is usted to be able to sort by a given field in the class.<br>
<br>
### Sorting in the Blazor Client
A component for sorting must be created and it will consist basically of a listbox where the columns available to sort by are listed. It is very simple. The value choosen here will be passed to the container through the typical `eventCallback` so the container can call the backend with this parameter. The trigger for this, will be the event `@onchange` of the `select control`. The component specify that when the value is changed a specific event in the `codebehind` should be also executed. In this case: `ApplySort`. This method simply gets the selected sorting field and passes it to the container through the callback. This will obviously imply that we create that new method in the container (the page). It will be also based on the initial `GetProducts` we created, but now receiving the sorting parameter. This parameter should be then passed to the service in the `query string` or whatever other method defined to the communication.
#### How does the Callback work?
The component has an event callback. This means that any container that implements the container can specify a local method that will be executed if the event of the component is raised:<br>
	`<Search OnSearchChanged="SearchChanged"/>`<br>
In this case, the component has an event callback called `OnSearchChanged` and the container has a method called `SearchChanged` that will be executed when the component trigger the event.<br>
How do you trigger (invoke) that event from the component?<br>
`await OnSortChanged.InvokeAsync(eventArgs.Value.ToString());`


