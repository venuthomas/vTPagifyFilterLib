# PagifyFilter
 Easy Pagination and Filter

### Overview
**PagifyFilter** is a NuGet package that provides an extension method for **IEnumerable<T>** to enable asynchronous searching, filtering, and ordering of collections based on the properties of a **PagifyFilter.Pagination** object. This package is designed to simplify and optimize the process of querying collections in .NET applications.

Here's how the original code can be optimized:
```
var searchResults = people
    .Where(dto => 
        (string.IsNullOrEmpty(searchName) || dto.Name?.Contains(searchName, StringComparison.OrdinalIgnoreCase) == true) &&
        (!searchAge.HasValue || dto.Age == searchAge) &&
        (string.IsNullOrEmpty(searchAddress) || dto.Address?.Contains(searchAddress, StringComparison.OrdinalIgnoreCase) == true)
    ).ToList();

// Sorting
var sortedResults = searchResults
    .OrderBy(dto => dto.Name)
    .ThenBy(dto => dto.Age)
    .ToList();
```
##### Optimized Code using PagifyFilter
```
var result = await people.SearchAsync(searchDto);
```

### Installation
These steps must be adhered to when using this package with **IEnumerable** collections.

To install the package, choose one of the following commands:
##### .NET CLI:
```
  dotnet add package vTPagifyFilterLib
```

##### Package manager:

```
Install-Package vTPagifyFilterLib
```

### Usage
The code snippet **public class YourSearchDto : PagifyFilter.Pagination { }** defines a class named **YourSearchDto** that inherits from a base class called **PagifyFilter.Pagination**. Here’s a breakdown of what this means:

#### Inheritance in C#
In C#, inheritance allows a class to inherit the properties and methods of another class. The class that inherits is called the derived class (in this case, **YourSearchDto**), and the class being inherited from is called the base class (in this case, **PagifyFilter.Pagination**).

#### PagifyFilter.Pagination Class
The **PagifyFilter.Pagination** class is likely a class that contains properties and methods related to paginating data. Common properties in a Pagination class might include:
     **CurrentPageNo**: The current page number.
     **ItemsPerPage**: The number of items per page.
     **TotalItems**: The total number of items.
     **SortColumn**: Column for Sorting.
     **IsAscendingSort**: If true , it means ascending otherwise descending

#### YourSearchDto Class
By inheriting from **PagifyFilter.Pagination**, the **YourSearchDto** class will have all the properties and methods of the **Pagination** class. This means that **YourSearchDto** can be used to handle pagination-related data along with any additional properties or methods you might add to it.

#### Integration with PagifyFilter
The **PagifyFilter** package provides an extension method **SearchAsync** for **IEnumerable<T>** to enable asynchronous searching, filtering, and ordering of collections based on the properties of a SearchDto object. Here’s how YourDto can be used with PagifyFilter:
Define YourDto: Inherit from Pagination to include pagination properties.
Use YourDto with SearchAsync: Pass an instance of YourDto to the SearchAsync method to filter and paginate your data.

### Example
Here’s an example to illustrate

##### Defining Your Search DTO
Create a class that inherits from **PagifyFilter.Pagination** to include pagination properties in your data transfer object (DTO): Note: all fields must be allowed to be null by using '?'.
```
public class YourSearchDto : PagifyFilter.Pagination
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public string? Address { get; set; }
}
```

##### Integrating with PagifyFilter
Use the **SearchAsync** extension method provided by PagifyFilter to filter and paginate your data
```
var searchDto = new YourSearchDto
    {
        PageNumber = 1,
        PageSize = 10,
        Name = "A"
    };

var result = await people.SearchAsync(searchDto);
```

##### Adding Sorting
If you need sorting, specify the column and sort direction:
```
var searchDto = new YourSearchDto
    {
        PageNumber = 1,
        PageSize = 10,
        Name = "A"
        SortColumn ="Age"
    };

var result = await people.SearchAsync(searchDto);
```

#### Return Value
The **SearchAsync** method returns a **PagedResult<Tdata>**, which contains the filtered and optionally ordered elements from the source collection

### Practical Application
In a real-world scenario, **YourSearchDto** can be used to pass search criteria and pagination information to a method that retrieves data from a database or API. This approach helps manage large datasets efficiently, improving performance and user experience.

### Summary
**PagifyFilter** simplifies pagination, filtering, and sorting in .NET applications.
Extend **PagifyFilter.Pagination** to include custom search criteria in your search DTO.
Use **SearchAsync** to efficiently retrieve and manage data.

If you have any more questions or need further clarification, feel free to ask!
