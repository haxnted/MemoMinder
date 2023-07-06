# MemoMinder: Ваш идеальный планировщик.
---
>__MemoMinder__ - это планировщик задач, который превосходит многие другие программы заметок своим потенциалом. Он предлагает ряд уникальных возможностей, которых часто не хватает другим приложениям подобного плана. Позвольте мне рассказать вам о его особенностях.

1. _Гибкость окна_:
MemoMinder позволяет полностью настроить внешний вид окна по вашему вкусу. Вы можете выбрать фон окна, фон текста, шрифт, цвет текста и многое другое. Это означает, что вы сможете создать идеальное окружение для работы с вашими заметками. Кроме того, вы можете сохранить свои настройки окна, чтобы каждая новая заметка соответствовала вашему стилю. Если вы предпочитаете максимальную концентрацию, вы также можете скрыть верхнюю панель окна Windows, чтобы ничто не отвлекало вас от вашей работы. И если вы всегда мечтали о создании совершенной заметки, то для вас доступно специальное окно настроек, где вы сможете воплотить это.
<br>
2. _Дополнительные окна_:
    MemoMinder предлагает удобное отображение всех заметок, находящихся в папке. Вы сможете легко просматривать и управлять своими заметками, не открывая каждую из них отдельно. Это сделает вашу работу более организованной и эффективной.

---
##Работа с приложением

C первым запуском окна вас будет встречать следующее окно:
<p align="center">
  <img src="https://i.imgur.com/ZxMya2f.png" />
</p>
Начальное окно выглядит слегка пугающим, потому что это стандартное окно, которое срабатывает при первом запуске приложения и при открытии новых окон, об этом, чуть будет ниже.

Разберем ключевые особенности окна.
1. _Верхняя панель_. Полностью переделанная панель windows, имеющая возможность скрываться при вашей необходимости при помощи сочетаний клавиш __`ctrl + S`__ и обратно. Включает в себя 5 основных кнопок:
    - 1 Кнопка. Показывает все существующие заметки в папке. Они хранятся по этому пути: `\AppData\Roaming\MemoMinder\Notes`
    - 2 Кнопка. Создает новое окно с заметкой. Для того, чтобы каждый раз не приходилось менять стиль окна при запуске приложения был создан файл `DefaultThemeNote.json`. Он хранит в себе данные для нового окна. О том как сохранять данные базового окна - рассмотрим в 4 кнопке "Настройки".
    - 3 Кнопка. Закрепление приложения. Порой, непривычно когда другие приложения закрывают приложение, из-за этого приходится каждый раз нажимать на приложение. Специально для этого и существует эта кнопка, которая закрепляет приложение поверх остальных окон, нажав второй раз по кнопке - приложение открепится.
    - 4 Кнопка. Настройки. Тут вы сможете изменить стиль окна, об параметрах узнаете ниже. 
    - 5 Кнопка. Удаление заметки. Удаляет файл без возможности восстановления. Вместо удаленного окна откроется рандомным образом другое окно.
2. _Заголовок заметки_. Здесь вы можете дать имя вашей заметке, чтобы быстро его найти в окне все заметки. Вы можете отключить этот текст в настройках, чтобы освободить место для текста.
3. _Текст заметки_. На момент версии приложения 1.0 Существует только две опции текста. Создание пунктов заметки и скроллинг вниз при помощи стрелок на клавиатуре. При помощи комбинации __*.-5*__ (где число 5 - кол-во пуктов, которые вы желаете создать) создается n кол-во пунктов. В будущих версиях будет больше функций!
<p align="center">
  <img src="[Imgur](https://i.imgur.com/qTaMnVl.png)" Height="300"/>
</p>

##Более детальное описание окон.
1. Окно заметок. Здесь отображены все ваши идеи и планы. Нажав на заметку отроектся соответствующее окно.
<p align="center">
  <img src="https://i.imgur.com/qTaMnVl.png" Height="300"/>
</p>
2. Настройки. Самое обширное окно, с множеством функций.
<p align="center">
  <img src="https://i.imgur.com/cRyNA01.png" Height="700"/>
</p>

##Расмотрим каждое поле.
>P.S. Во избежаний проблем, не рекомендуется оставлять пустые поля настроек.
__Background window__. Фон окна. Если вы желаете сменить однотонный цвет, на какой-нибудь другой, писать нужно на английском языке, например: Red, LightBlue, DarkGreen_(Ознакомится со всеми доступными однотонными цветами можете по [ссылке](https://learn.microsoft.com/ru-ru/dotnet/api/system.drawing.brushes?view=windowsdesktop-3.1))__. Но вы можете применить к фону ваше любимое фото. Для того, чтобы удобнее было добавлять фотографию, нажмите два раза по тексту в 1 импуте. Откроется окно с базовым расположением всех фонов заметок, вы можете перейти в другую директорию и выбрать вашу фотографию, в следующий раз при открытии этого окна, она продублируется в папку *Backgrounds*. Измененный цвет сразу же отобразиться правее от поля.
__Background text__. Фон текста. Если вы желаете применить 2 фона, вы смело это сможете сделать. Также поддерживается как и цвет, так и фотографиия с возможностью выбрать как в свойстве выше. Если вы желаете сделать фон прозрачным, впишите следующий текст `Transparent` и фон текста станет прозрачным (не рекомендуется применять Transparent с фону ОКНА, то есть к Background window).
__Caption foreground__. Изменения цвета шрифта заголовка. Строго писать доступными цветами, по типу Red, Black. Продублирую список доступных цветов: [ссылка](https://learn.microsoft.com/ru-ru/dotnet/api/system.drawing.brushes?view=windowsdesktop-3.1).
__Foreground text__. Цвет непосредственно текста. Также нужно соблюдать доступные цвета.
__Caption font family__. Шрифт заголовка.
__Text font family__. Шрифт текста.
__Font size caption__. Размер шрифта заголовка.
__Font size text__. Размер шрифта текста.
__Margin text__. Отступ краев текста от окна.
__Window height, window width__. Размеры окна.
__Vertical scrollBar Visibility__. Добавляет вертикальную прокрутку, чтобы не управлять стрелками с клавиатуры.
__Toggle window__. Закрепляет окно поверх других. Только в этом случае сохрает еще и в файл. То есть при открытии приложения, заметка уже будет закреплена поверх других приложений.
__Show caption memo__. Включить / выключить заголовок заметки.
__Underline caption__. Добавляет нижнее подчеркивание заметке, чтобы лучше было видно текст.
__Save for default window__. Сохраняет данные окна в БАЗОВЫЙ ФАЙЛ, то есть при открытии нового окна, к этому окну применится стиль который вы применяли окну.
---
#####По вопросам обращайтесь через почту haxnt3d@gmail.com

