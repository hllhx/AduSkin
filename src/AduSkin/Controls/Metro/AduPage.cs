using AduSkin.Controls.Data;
using AduSkin.Controls.Tools.Extension;
using AduSkin.Interactivity;
using AduSkin.Utility.Element;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AduSkin.Controls.Metro
{
   /// <summary>
   ///     页码
   /// </summary>
   [TemplatePart(Name = ElementButtonLeft, Type = typeof(AduPathIconButton))]
   [TemplatePart(Name = ElementButtonRight, Type = typeof(AduPathIconButton))]
   [TemplatePart(Name = ElementButtonFirst, Type = typeof(RadioButton))]
   [TemplatePart(Name = ElementTextBlockLeft, Type = typeof(TextBlock))]
   [TemplatePart(Name = ElementPanelMain, Type = typeof(Panel))]
   [TemplatePart(Name = ElementTextBlockRight, Type = typeof(TextBlock))]
   [TemplatePart(Name = ElementButtonLast, Type = typeof(RadioButton))]
   public class AduPage : Control
   {
      static AduPage()
      {
         ElementBase.DefaultStyle<AduPage>(DefaultStyleKeyProperty);
      }

      #region Constants

      private const string ElementButtonLeft = "PART_ButtonLeft";
      private const string ElementButtonRight = "PART_ButtonRight";
      private const string ElementButtonFirst = "PART_ButtonFirst";
      private const string ElementTextBlockLeft = "PART_TextBlockLeft";
      private const string ElementPanelMain = "PART_PanelMain";
      private const string ElementTextBlockRight = "PART_TextBlockRight";
      private const string ElementButtonLast = "PART_ButtonLast";

      #endregion Constants

      #region Data

      private Button _buttonLeft;
      private Button _buttonRight;
      private RadioButton _buttonFirst;
      private TextBlock _textBlockLeft;
      private Panel _panelMain;
      private TextBlock _textBlockRight;
      private RadioButton _buttonLast;

      private bool _appliedTemplate;

      #endregion Data

      #region Public Events

      /// <summary>
      ///     页面更新事件
      /// </summary>
      public static readonly RoutedEvent PageUpdatedEvent =
          EventManager.RegisterRoutedEvent("PageUpdated", RoutingStrategy.Bubble,
              typeof(EventHandler<FunctionEventArgs<int>>), typeof(AduPage));

      /// <summary>
      ///     页面更新事件
      /// </summary>
      public event EventHandler<FunctionEventArgs<int>> PageUpdated
      {
         add => AddHandler(PageUpdatedEvent, value);
         remove => RemoveHandler(PageUpdatedEvent, value);
      }

      #endregion Public Events

      public AduPage()
      {
         Utility.Refresh(this);
         CommandBindings.Add(new CommandBinding(ControlCommands.Prev, ButtonPrev_OnClick));
         CommandBindings.Add(new CommandBinding(ControlCommands.Next, ButtonNext_OnClick));
         CommandBindings.Add(new CommandBinding(ControlCommands.Selected, ToggleButton_OnChecked));
         this.Show(MaxPageCount > 1);
      }

      #region Public Properties

      #region MaxPageCount

      /// <summary>
      ///     最大页数
      /// </summary>
      public static readonly DependencyProperty MaxPageCountProperty = DependencyProperty.Register(
          "MaxPageCount", typeof(int), typeof(AduPage), new PropertyMetadata(ValueBoxes.Int1Box, (o, args) =>
          {
             if (o is AduPage AduPage && args.NewValue is int value)
             {
                if (AduPage.PageIndex > AduPage.MaxPageCount)
                {
                   AduPage.PageIndex = AduPage.MaxPageCount;
                }

                AduPage.Show(value > 1);
                AduPage.Update();
             }
          }, (o, value) =>
          {
             if (!(o is AduPage)) return 1;
             var intValue = (int)value;
             if (intValue < 1)
             {
                return 1;
             }
             return intValue;
          }));

      /// <summary>
      ///     最大页数
      /// </summary>
      public int MaxPageCount
      {
         get => (int)GetValue(MaxPageCountProperty);
         set => SetValue(MaxPageCountProperty, value);
      }

      #endregion MaxPageCount

      #region DataCountPerPage

      /// <summary>
      ///     每页的数据量
      /// </summary>
      public static readonly DependencyProperty DataCountPerPageProperty = DependencyProperty.Register(
          "DataCountPerPage", typeof(int), typeof(AduPage), new PropertyMetadata(20, (o, args) =>
          {
             if (o is AduPage AduPage)
             {
                AduPage.Update();
             }
          }, (o, value) =>
          {
             if (!(o is AduPage)) return 1;
             var intValue = (int)value;
             if (intValue < 1)
             {
                return 1;
             }
             return intValue;
          }));

      /// <summary>
      ///     每页的数据量
      /// </summary>
      public int DataCountPerPage
      {
         get => (int)GetValue(DataCountPerPageProperty);
         set => SetValue(DataCountPerPageProperty, value);
      }

      #endregion

      #region PageIndex

      /// <summary>
      ///     当前页
      /// </summary>
      public static readonly DependencyProperty PageIndexProperty = DependencyProperty.Register(
          "PageIndex", typeof(int), typeof(AduPage), new PropertyMetadata(ValueBoxes.Int1Box, (o, args) =>
          {
             if (o is AduPage AduPage && args.NewValue is int value)
             {
                AduPage.Update();
                AduPage.RaiseEvent(new FunctionEventArgs<int>(PageUpdatedEvent, AduPage)
                {
                   Info = value
                });
             }
          }, (o, value) =>
          {
             if (!(o is AduPage AduPage)) return 1;
             var intValue = (int)value;
             if (intValue < 0)
             {
                return 0;
             }
             if (intValue > AduPage.MaxPageCount) return AduPage.MaxPageCount;
             return intValue;
          }));

      /// <summary>
      ///     当前页
      /// </summary>
      public int PageIndex
      {
         get => (int)GetValue(PageIndexProperty);
         set => SetValue(PageIndexProperty, value);
      }

      #endregion PageIndex

      #region MaxPageInterval

      /// <summary>
      ///     表示当前选中的按钮距离左右两个方向按钮的最大间隔（4表示间隔4个按钮，如果超过则用省略号表示）
      /// </summary>       
      public static readonly DependencyProperty MaxPageIntervalProperty = DependencyProperty.Register(
          "MaxPageInterval", typeof(int), typeof(AduPage), new PropertyMetadata(3, (o, args) =>
          {
             if (o is AduPage AduPage)
             {
                AduPage.Update();
             }
          }), value =>
          {
             var intValue = (int)value;
             return intValue >= 0;
          });

      /// <summary>
      ///     表示当前选中的按钮距离左右两个方向按钮的最大间隔（4表示间隔4个按钮，如果超过则用省略号表示）
      /// </summary>   
      public int MaxPageInterval
      {
         get => (int)GetValue(MaxPageIntervalProperty);
         set => SetValue(MaxPageIntervalProperty, value);
      }

      #endregion MaxPageInterval

      #endregion

      #region Public Methods

      public override void OnApplyTemplate()
      {
         _appliedTemplate = false;
         base.OnApplyTemplate();

         _buttonLeft = GetTemplateChild(ElementButtonLeft) as Button;
         _buttonRight = GetTemplateChild(ElementButtonRight) as Button;
         _buttonFirst = GetTemplateChild(ElementButtonFirst) as RadioButton;
         _textBlockLeft = GetTemplateChild(ElementTextBlockLeft) as TextBlock;
         _panelMain = GetTemplateChild(ElementPanelMain) as Panel;
         _textBlockRight = GetTemplateChild(ElementTextBlockRight) as TextBlock;
         _buttonLast = GetTemplateChild(ElementButtonLast) as RadioButton;

         CheckNull();

         _appliedTemplate = true;
         Update();
      }

      #endregion Public Methods

      #region Private Methods

      private void CheckNull()
      {
         if (_buttonLeft == null || _buttonRight == null || _buttonFirst == null ||
             _textBlockLeft == null || _panelMain == null || _textBlockRight == null ||
             _buttonLast == null) throw new Exception();
      }

      /// <summary>
      ///     更新
      /// </summary>
      private void Update()
      {
         if (!_appliedTemplate) return;
         _buttonLeft.IsEnabled = PageIndex > 1;
         _buttonRight.IsEnabled = PageIndex < MaxPageCount;
         if (MaxPageInterval == 0)
         {
            _buttonFirst.Collapse();
            _buttonLast.Collapse();
            _textBlockLeft.Collapse();
            _textBlockRight.Collapse();
            _panelMain.Children.Clear();
            var selectButton = CreateButton(PageIndex);
            _panelMain.Children.Add(selectButton);
            selectButton.IsChecked = true;
            return;
         }
         _buttonFirst.Show();
         _buttonLast.Show();
         _textBlockLeft.Show();
         _textBlockRight.Show();

         //更新最后一页
         if (MaxPageCount == 1)
         {
            _buttonLast.Collapse();
         }
         else
         {
            _buttonLast.Show();
            _buttonLast.Tag = MaxPageCount.ToString();
         }

         //更新省略号
         var right = MaxPageCount - PageIndex;
         var left = PageIndex - 1;
         _textBlockRight.Show(right > MaxPageInterval);
         _textBlockLeft.Show(left > MaxPageInterval);

         //更新中间部分
         _panelMain.Children.Clear();
         if (PageIndex > 1 && PageIndex < MaxPageCount)
         {
            var selectButton = CreateButton(PageIndex, true);
            _panelMain.Children.Add(selectButton);
            selectButton.IsChecked = true;
            _buttonFirst.IsChecked = false;
            _buttonLast.IsChecked = false;
         }
         else if (PageIndex == 1)
         {
            _buttonFirst.IsChecked = true;
         }
         else
         {
            _buttonLast.IsChecked = true;
         }

         var sub = PageIndex;
         for (var i = 0; i < MaxPageInterval - 1; i++)
         {
            if (--sub > 1)
            {
               _panelMain.Children.Insert(0, CreateButton(sub));
            }
            else
            {
               break;
            }
         }
         var add = PageIndex;
         for (var i = 0; i < MaxPageInterval - 1; i++)
         {
            if (++add < MaxPageCount)
            {
               _panelMain.Children.Add(CreateButton(add));
            }
            else
            {
               break;
            }
         }
      }

      private void ButtonPrev_OnClick(object sender, RoutedEventArgs e) => PageIndex--;

      private void ButtonNext_OnClick(object sender, RoutedEventArgs e) => PageIndex++;

      private AduRadioButton CreateButton(int page, bool isSelected = false)
      {
         return new AduRadioButton
         {
            Content = page.ToString(),
            Tag = page.ToString(),
            IconVisibility = Visibility.Collapsed,
            Opacity = isSelected ? 1 : 0.8,
            FontWeight = isSelected ? FontWeight.FromOpenTypeWeight(600) : FontWeight.FromOpenTypeWeight(400),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Center
         };
      }

      private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
      {
         if (!(e.OriginalSource is RadioButton button)) return;
         if (button.IsChecked == false) return;
         PageIndex = int.Parse(button.Tag.ToString());
      }

      #endregion Private Methods       
   }
}
