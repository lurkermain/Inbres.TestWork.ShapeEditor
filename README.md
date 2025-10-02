Тестовое задание в ИНБРЭС "Простой редактор фигур".
----------------------------
#### Используемый стэк:
1. Технологии и фреймворки: .net8 или выше, C#12 или выше, Avalonia UI, Reactive UI или CommunityToolkit.Mvvm;
2. Архитектурные паттерны: MVVM, [FSD](https://feature-sliced.github.io/documentation/ru/docs/get-started/overview), [Clean Architecture](https://www.youtube.com/watch?v=WlCDcr8JYFU);

## Основное задание.
Требуется разработать редактор фигур на Avalonia UI, для реализации выбрать один примитив и одну кривую из предлагаемых фигур.

Особенность примитивов в том, что они состоят из фиксированного числа точек, в свою очередь количество опорных точек в кривой линии может изменяться, необходимо предусмотреть возможность добавления новых точек в кривую линию на момент ее создания. Аналогично примитивам необходима возможность изменения их размеров хотя бы на момент создания фигуры.

Необходимо продемонстрировать работу с паттернами MVVM, сериализовав ViewModel или Model фигур, с последующим востановлением состояния при новой загрузке проекта. В сериализуемые данные не должны попадать Control'ы Avalonia Ui, сериализация структур Point, Color, Thickness, а так же иных структур и классов не наследующихся от Сontrol вполне допустима. 

Для простоты реализации фигур можно использовать [Path](https://docs.avaloniaui.net/ru/docs/guides/graphics-and-animation/graphics-and-animations) либо отрисовывать через [DrawingContext.DrawGeometry](https://reference.avaloniaui.net/api/Avalonia.Media/DrawingContext/E76A87CD). PathGeometry уже имеет функционал для отрисовки кривых Безье, можно ореинтироваться на документацию по [svg](https://developer.mozilla.org/ru/docs/Web/SVG/Tutorials/SVG_from_scratch/Paths) path, их синтаксис довольно схож.


#### Предлагаемые фигуры:
1. Примитивы: Прямоугольник, Овал, Треугольник.
2. Кривые: Квадратичная кривая Безье, Кубическая кривая Безье.

## Требование к решению:

####  Версионирование.
1. Необходимо сделать форк данного репозитория и разместить свое решение в нем.
2. По мере реализации фич необходимо делать фиксации в git.
3. После завершения тестового задания необходимо предложить слияние в [родительский репозиторий](https://github.com/Andreev-Da/Inbres.TestWork.ShapeEditor).

#### Структура решения.
1. sln файл должен располагаться в [корневой директории](.), а сами проекты в директории [Source](Source)
2. Решение необходимо поделить минимум на два проекта, один с исходным кодом редактора, второй с запускаемым приложением.
3. Редактор должен интегрироваться в запускаемое приложение как UserControl или TemplatedControl (на ваш выбор).

#### Требования к коду.
1. Необходимо использовать [соглашения по наименованию от Microsoft.](https://learn.microsoft.com/ru-ru/dotnet/standard/design-guidelines/general-naming-conventions)
2. Код должен быть [Native AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=windows%2Cnet8) совместим.
   <br>Для сохранения горячей перезагрузки, флаг PublishAot устанавливается только на этапе публикации решения, поэтому для получения предупреждений от компилятор в csproj необходимо добавить флаг IsAotCompatible.
3. Для реализации привязки данных необходимо использовать ReactiveUI либо CommunityToolkit.Mvvm.


#### Архитектурные требования.
1. При создании компонентов редактора необходимо ориентироваться на практику MVVM и методологию [FSD](https://feature-sliced.github.io/documentation/ru/docs/get-started/overview), так как проект не особо сложный предлагается следующая файловая структура без деления на сегменты (ui, model и т.п.):
   <br>Пример расположение компонента ShapesEditorWidget: \Widgets\ShapesEditor\ShapesEditorWidget.axaml
   <br>Для его ViewModel расположение будет схожим: \Widgets\ShapesEditor\ShapesEditorWidgetModel.cs

![предлагаемое расположение компонентов](Assets/ComponentsArchitecture.png)

#### Публикация
1. Решение должно быть собрано c флагом Native AOT в конфигурации Release под linux и windows, после чего размешено в github Releases вашего репозитория.
   <br> `dotnet publish -p:PublishAot=true -r win-x64 -c Release`
   <br> `dotnet publish -p:PublishAot=true -r linux-arm64 -c Release`


## Рекомендации
Используйте [HotAvalonia](https://github.com/Kira-NT/HotAvalonia) для горячей перезагрузки axaml.

## Пример решения: 
![Sample video](/Assets/Sample.gif)
_*От вас не требуется весь представленный функционал._
