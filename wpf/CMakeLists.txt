cmake_minimum_required(VERSION 3.20)

project(wpf LANGUAGES CSharp)

# Подключаем модуль с утилитами для C#
include(CSharpUtilities)

# Функция для рекурсивного сбора файлов
function(recursive_glob VAR STARTING_DIR)
    file(GLOB_RECURSE CS_FILES "${STARTING_DIR}/*.cs")
    file(GLOB_RECURSE XAML_FILES "${STARTING_DIR}/*.xaml")
    file(GLOB RESOURCE_FILES "Resources/*.resx")
    set(ALL_FILES ${CS_FILES} ${XAML_FILES} ${RESOURCE_FILES})
    list(REMOVE_DUPLICATES ALL_FILES)
    
    # Исключаем системные папки
    list(FILTER ALL_FILES EXCLUDE REGEX "[/\\\\](bin|obj|build)[/\\\\].*")
    
    # Исключаем автоматически генерируемые файлы
    list(FILTER ALL_FILES EXCLUDE REGEX "AssemblyInfo\\.cs$")
    list(FILTER ALL_FILES EXCLUDE REGEX "\\.g\\.cs$")
    list(FILTER ALL_FILES EXCLUDE REGEX "\\.g\\.i\\.cs$")
    
    # ★★★ ИСКЛЮЧАЕМ ЛИШНИЕ ТОЧКИ ВХОДА ★★★
    # Исключаем Program.cs если есть
    list(FILTER ALL_FILES EXCLUDE REGEX "Program\\.cs$")
    # Исключаем Startup.cs если есть
    list(FILTER ALL_FILES EXCLUDE REGEX "Startup\\.cs$")
    # Исключаем любой файл, который может содержать Main()
    list(FILTER ALL_FILES EXCLUDE REGEX "EntryPoint\\.cs$")
    list(FILTER ALL_FILES EXCLUDE REGEX "Main\\.cs$")
    
    set(${VAR} ${ALL_FILES} PARENT_SCOPE)
endfunction()

recursive_glob(PROJECT_SOURCES ${CMAKE_CURRENT_SOURCE_DIR})

# Выводим список для отладки
message(STATUS "Found source files:")
foreach(FILE ${PROJECT_SOURCES})
    message(STATUS "  ${FILE}")
endforeach()

#configure_file(wpf.csproj ${CMAKE_CURRENT_BINARY_DIR}/wpf.csproj COPYONLY)

# Добавляем исполняемый файл
add_executable(wpf WIN32 ${PROJECT_SOURCES}) #без консоли
#add_executable(wpf ${PROJECT_SOURCES}) #с кнсолью


# Восстановление NuGet пакетов перед сборкой
# set( SOLUTION_PATH "${CMAKE_CURRENT_SOURCE_DIR}/YourSolution.sln" )
# add_custom_command(
#     TARGET wpf PRE_BUILD
#     #COMMAND dotnet restore "${SOLUTION_PATH}"
#     COMMAND dotnet restore "${CMAKE_CURRENT_SOURCE_DIR}/YourProject.csproj"
# )
# Добавляем NuGet-пакет с актуальными Behaviors
find_package(Git REQUIRED)
execute_process(
    COMMAND dotnet add ${CMAKE_CURRENT_SOURCE_DIR}/wpf.csproj package Microsoft.Xaml.Behaviors.Wpf
    WORKING_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}
    RESULT_VARIABLE dotnet_result
)

# Настройки для .NET 8.0 WPF
set_target_properties(wpf PROPERTIES
    DOTNET_SDK "Microsoft.NET.Sdk"
    DOTNET_TARGET_FRAMEWORK "net8.0-windows"
    #VS_GLOBAL_StartupObject "wpf.App"  # Явно указываем App как точку входа
    VS_GLOBAL_UseWPF "true"
    VS_GLOBAL_OutputType "WinExe"
    VS_GLOBAL_Nullable "enable"
    VS_GLOBAL_DisableWinExeOutputInference "true"  
    VS_PACKAGE_REFERENCES "Microsoft.Xaml.Behaviors.Wpf_1.1.135"
    VS_PACKAGE_REFERENCES "Newtonsoft.Json_13.0.3"
    VS_GLOBAL_UseWindowsForms "true"
    VS_GLOBAL_RootNamespace "WpfApp"
)

# App.xaml как точка входа
if(EXISTS "${CMAKE_CURRENT_SOURCE_DIR}/App.xaml")
    set_property(SOURCE App.xaml PROPERTY VS_XAML_TYPE "ApplicationDefinition")
else()
    message(WARNING "App.xaml not found!")
endif()


# Для каждого XAML файла (кроме App.xaml) устанавливаем тип Page
# И связываем с .xaml.cs файлом
foreach(XAML_FILE ${ALL_XAML})
    get_filename_component(XAML_NAME ${XAML_FILE} NAME)
    
    if(NOT XAML_NAME STREQUAL "App.xaml")
        # Устанавливаем тип Page
        set_property(SOURCE "${XAML_FILE}" PROPERTY VS_XAML_TYPE "Page")
        
        # Находим соответствующий .xaml.cs файл
        string(REGEX REPLACE "\\.xaml$" ".xaml.cs" CS_FILE "${XAML_FILE}")
        if(EXISTS "${CS_FILE}")
            # Связываем XAML с code-behind
            set_property(SOURCE "${XAML_FILE}" PROPERTY 
                VS_XAML_DEPENDENT_UPON "${CS_FILE}")
        endif()
    endif()
endforeach()

# Добавляем команду для установки пакета
add_custom_command(
    TARGET wpf PRE_BUILD
    COMMAND dotnet add ${CMAKE_CURRENT_SOURCE_DIR}/wpf.csproj package Microsoft.Xaml.Behaviors.Wpf --version 1.1.135
    COMMAND dotnet restore ${CMAKE_CURRENT_SOURCE_DIR}/wpf.csproj
    WORKING_DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}
    COMMENT "Adding Microsoft.Xaml.Behaviors.Wpf package..."
)

# КЛЮЧЕВОЙ МОМЕНТ: автоматически настраиваем свойства для .resx файлов
csharp_set_designer_cs_properties(${RESOURCE_FILES})