using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToDo.Models;
using ToDo.Models.ViewModels;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;


namespace ToDo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var todoListViewModel = GetAllTodos();
        return View(todoListViewModel);
    }

    [HttpGet]
    public JsonResult PopulateForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }

    internal TodoItem GetById(int id)
    {
        TodoItem todo = new();

        using (SqliteConnection con =
        new SqliteConnection("Data Source=db.sqlite"))
        {
            using(var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"SELECT * FROM todo WHERE Id='{id}'";

                using(var reader = tableCmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        reader.Read();
                        todo.Id = reader.GetInt32(0);
                        todo.Name = reader.GetString(1);
                    }
                    else{
                        return todo;
                    }
                };
            }
        }
        return todo;

    }

    internal TodoViewModel GetAllTodos()
    {
        List<TodoItem> todoList = new();

        using (SqliteConnection con =
        new SqliteConnection("Data Source=db.sqlite"))
        {
            using(var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = "SELECT * FROM todo";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            todoList.Add(
                                new TodoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                        }
                    }
                    else
                    {
                        return new TodoViewModel
                        {
                            TodoList = todoList
                        };
                    }
                }
            }
        }
        return new TodoViewModel
        {
            TodoList = todoList
        };

    }

    public RedirectResult Insert(TodoItem todo)
    {
        using (SqliteConnection con =
        new SqliteConnection("Data Source=db.sqlite"))
        {
            using(var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5126/");
    }


    public JsonResult Delete(int id)
    {
        using (SqliteConnection con = 
        new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"DELETE FROM todo WHERE Id = '{id}'";
                tableCmd.ExecuteNonQuery();
            }

            return Json(new{});
        }
    }

    public RedirectResult Update(TodoItem todo)
    {
        using (SqliteConnection con =
        new SqliteConnection("Data Source=db.sqlite"))
        {
            using(var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"UPDATE todo SET name = '{todo.Name}' WHERE Id = '{todo.Id}'";
                try
                {
                    tableCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    
                    Console.WriteLine(ex.Message);
                }
            }
        }
        return Redirect("http://localhost:5126/");

    }
}
