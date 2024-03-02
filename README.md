# MyBook: Simple Book Management Web API
<img src="https://github.com/emaadgh/mybook/assets/10380342/8c04de39-c177-4dd8-866e-8fa6efd7e98d" width="150" height="150">

**MyBook** is a straightforward **ASP.NET Core Web API** project designed for managing book-related data. Whether you're building a personal library or creating a book catalog for an application, MyBook provides the necessary endpoints to handle book CRUD operations.

## API Endpoints

The MyBook API provides the following endpoints:

### Authors

- **GET /api/Authors**: Retrieves an author by ID. Optional query parameters include `id` and `fields`.
- **POST /api/Authors**: Creates a new author. The request body should include the author details.
- **PUT /api/Authors**: Updates an existing author. The request body should include the updated author details.
- **PATCH /api/Authors**: Partially updates an author. The request body should include the fields to update and their new values.
- **DELETE /api/Authors**: Deletes an author by ID.

### Books

- **GET /api/Books**: Retrieves a book by ID. Optional query parameters include `id` and `fields`.
- **POST /api/Books**: Creates a new book for an author. The request body should include the book details.
- **PUT /api/Books**: Updates an existing book. The request body should include the updated book details.
- **PATCH /api/Books**: Partially updates a book. The request body should include the fields to update and their new values.
- **DELETE /api/Books**: Deletes a book by ID.

For more details about the request and response formats, please refer to the Swagger documentation.

## Getting Started

1. **Clone the Repository**:
    ```bash
    git clone https://github.com/emaadgh/mybook.git
    ```

2. **Install Dependencies**:
    - Ensure you have **.NET SDK** installed.
    - Open a terminal and navigate to the project folder:
        ```bash
        cd mybook
        ```
    - Restore dependencies:
        ```bash
        dotnet restore
        ```

3. **Run the Application**:
    - Start the API:
        ```bash
        dotnet run
        ```
    - The API will be available at `https://localhost:5001/api/books`.

4. **Explore the API**:
    - Use tools like **Swagger** or **Postman** to interact with the endpoints.
    - Visit the Swagger UI at `https://localhost:5001/swagger/index.html`.

## Contributing

Contributions are welcome! If you'd like to enhance MyBook, feel free to submit pull requests or open issues.

## License

This project is licensed under the MIT License.
