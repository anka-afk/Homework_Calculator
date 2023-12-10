using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Homework_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Page2 : Page
    {
        static string[] symbolsToRemove = { "sin", "cos", "log", "ln", "tan", "abs" };
        public string record = "";
        public Page2()
        {
            InitializeComponent();

        }


        private void JumpToHistory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string historyItem)
            {
                // 查找等号的位置
                int equalsIndex = historyItem.IndexOf("=");

                if (equalsIndex != -1)
                {
                    // 剪切掉等号及其之后的内容
                    string expression = historyItem.Substring(0, equalsIndex);

                    // 将剪切后的表达式应用到 TextBox
                    textBox.Text = expression;
                }
                else
                {
                    // 如果没有等号，则直接应用整个历史记录
                    textBox.Text = historyItem;
                }

            }
        }

        public static void item_divide(string input, out string[] items, out int sum)
        {
            string expression = "";
            items = new string[233];
            sum = 0;
            char temp;
            input += "#";
            int couples = 0;
            for (int i = 0; input[i] != '#'; i++)
            {
                temp = input[i];
                if (temp >= '0' && temp <= '9' && (input[i + 1] >= 'a' && input[i + 1] <= 'z' && (input[i + 1] != 'l' && input[i + 2] != 'o' && input[i + 3] != 'g') || input[i + 1] == '('))
                {
                    expression = expression + temp + '*';
                }
                else if (temp >= 'a' && temp <= 'z' && input[i + 1] >= 'a' && input[i + 1] <= 'z')
                {
                    string alpha = "";
                    alpha += temp;
                    while (input[i + 1] >= 'a' && input[i + 1] <= 'z')
                    {
                        alpha += input[i + 1];
                        i++;
                    }
                    while (alpha.Length != 0)
                    {
                        switch (alpha)
                        {
                            case "sin":
                                expression += "sin";
                                alpha = "";
                                break;
                            case "cos":
                                expression += "cos";
                                alpha = "";
                                break;
                            case "tan":
                                expression += "tan";
                                alpha = "";
                                break;
                            case "abs":
                                expression += "abs";
                                alpha = "";
                                break;
                            case "lg":
                                expression += "lg";
                                alpha = "";
                                break;
                            case "log":
                                expression += "$";
                                alpha = "";
                                break;
                            case "ln":
                                expression += "lg";
                                alpha = "";
                                break;
                            case "√":
                                expression += "√";
                                alpha = "";
                                break;
                            case "!":
                                expression += "!";
                                alpha = "";
                                break;

                        }
                    }
                }
                else if (temp == '(')
                {
                    expression += temp;
                    couples++;
                    if (input[i + 1] == '-' && (input[i + 2] >= 'a' && input[i + 2] <= 'z'))
                    {
                        expression += "0-1*";
                        i++;
                    }
                    else if (input[i + 1] == '-' && (input[i + 2] >= '0' && input[i + 2] <= '9'))
                    {
                        expression += "0-";
                        i++;
                    }
                }
                else if (temp == ')')
                {
                    expression += temp;
                    couples--;
                }
                else
                {
                    if (couples == 0 && (temp == '+' || temp == '-'))
                    {
                        items[sum] = expression;
                        sum++;
                        expression = Convert.ToString(temp);
                    }
                    else
                    {
                        expression += temp;
                    }
                }
            }
            items[sum] = expression;
            sum++;
        }

        public static double Caculate(string[] items, int sum)
        {
            Stack<double> data = new Stack<double>();
            Stack<string> ope = new Stack<string>();
            double result = 0;
            char temp;
            string alpha;
            int flag = 0;
            while (sum > 0)
            {
                alpha = "";
                items[--sum] += '#';
                data.Clear();
                ope.Clear();
                data.Push(0); // 栈底元素为0，便于处理负数 
                for (int i = 0; items[sum][i] != '#'; i++)
                {
                    temp = items[sum][i];
                    alpha += temp;



                    if (temp >= '0' && temp <= '9')
                    {
                        while (items[sum][i] != '#')
                        {
                            if (items[sum][i + 1] >= '0' && items[sum][i + 1] <= '9' || items[sum][i + 1] == '.')
                            {
                                alpha += items[sum][i + 1];
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        data.Push(Convert.ToDouble(alpha));
                    }

                    // 在 else if 中添加处理根号的逻辑
                    else if (temp == '√')
                    {
                        // 处理根号，将根号视为一个操作数
                        alpha = "";
                        while (items[sum][i + 1] != '#' && (char.IsDigit(items[sum][i + 1]) || items[sum][i + 1] == '.'))
                        {
                            alpha += items[sum][++i];
                        }

                        // 处理根号中的表达式
                        double operand;
                        if (!string.IsNullOrEmpty(alpha) && double.TryParse(alpha, out operand))
                        {
                            if (items[sum][i + 1] == '(')
                            {
                                // 处理括号内的表达式
                                string subExpression = "";
                                int openParentheses = 1;
                                while (openParentheses > 0 && items[sum][i + 1] != '#')
                                {
                                    char nextChar = items[sum][++i];
                                    subExpression += nextChar;
                                    if (nextChar == '(')
                                    {
                                        openParentheses++;
                                    }
                                    else if (nextChar == ')')
                                    {
                                        openParentheses--;
                                    }
                                }
                                // 计算括号内的表达式
                                double subExpressionResult = Caculate(subExpression.Split(' '), subExpression.Split(' ').Length);
                                // 将括号内的计算结果应用到根号操作
                                data.Push(Math.Sqrt(subExpressionResult));
                            }
                            else
                            {
                                // 不涉及括号的情况，直接应用根号
                                data.Push(Math.Sqrt(operand));
                            }
                        }
                        else
                        {
                            // alpha 不是合法的数字，可以在这里进行错误处理
                        }
                    }

                    // 在 else if 中添加处理幂次的逻辑
                    else if (temp == '^')
                    {
                        // 处理幂次，将幂次视为一个操作数
                        alpha = "";
                        while (items[sum][i + 1] != '#' && (char.IsDigit(items[sum][i + 1]) || items[sum][i + 1] == '.'))
                        {
                            alpha += items[sum][++i];
                        }

                        // 处理幂次中的表达式
                        double exponent;
                        if (!string.IsNullOrEmpty(alpha) && double.TryParse(alpha, out exponent))
                        {
                            if (items[sum][i + 1] == '(')
                            {
                                // 处理括号内的表达式
                                string subExpression = "";
                                int openParentheses = 1;
                                while (openParentheses > 0 && items[sum][i + 1] != '#')
                                {
                                    char nextChar = items[sum][++i];
                                    subExpression += nextChar;
                                    if (nextChar == '(')
                                    {
                                        openParentheses++;
                                    }
                                    else if (nextChar == ')')
                                    {
                                        openParentheses--;
                                    }
                                }
                                // 计算括号内的表达式
                                double subExpressionResult = Caculate(subExpression.Split(' '), subExpression.Split(' ').Length);
                                // 将括号内的计算结果应用到幂次操作
                                data.Push(Math.Pow(subExpressionResult, exponent));
                            }
                            else
                            {
                                // 不涉及括号的情况，直接应用幂次
                                data.Push(Math.Pow(data.Pop(), exponent));
                            }
                        }
                        else
                        {
                            // alpha 不是合法的数字，可以在这里进行错误处理
                        }
                    }

                    // 在 else if 中添加处理余数的逻辑
                    else if (temp == '%')
                    {
                        // 处理余数，将余数视为一个操作数
                        alpha = "";
                        while (items[sum][i + 1] != '#' && (char.IsDigit(items[sum][i + 1]) || items[sum][i + 1] == '.'))
                        {
                            alpha += items[sum][++i];
                        }

                        // 处理余数中的表达式
                        double divisor;
                        if (!string.IsNullOrEmpty(alpha) && double.TryParse(alpha, out divisor) && divisor != 0)
                        {
                            if (items[sum][i + 1] == '(')
                            {
                                // 处理括号内的表达式
                                string subExpression = "";
                                int openParentheses = 1;
                                while (openParentheses > 0 && items[sum][i + 1] != '#')
                                {
                                    char nextChar = items[sum][++i];
                                    subExpression += nextChar;
                                    if (nextChar == '(')
                                    {
                                        openParentheses++;
                                    }
                                    else if (nextChar == ')')
                                    {
                                        openParentheses--;
                                    }
                                }
                                // 计算括号内的表达式
                                double subExpressionResult = Caculate(subExpression.Split(' '), subExpression.Split(' ').Length);
                                // 将括号内的计算结果应用到余数操作
                                data.Push(subExpressionResult % divisor);
                            }
                            else
                            {
                                // 不涉及括号的情况，直接应用余数
                                data.Push(data.Pop() % divisor);
                            }
                        }
                        else
                        {
                            // alpha 不是合法的数字，或除数为零，可以在这里进行错误处理
                        }
                    }

                    // 在处理阶乘的代码中添加调试信息
                    // 处理阶乘的逻辑
                    else if (temp == '!')
                    {
                        // 向前读取操作数
                        alpha = "";
                        int j = i - 1;
                        while (j >= 0 && char.IsDigit(items[sum][j]))
                        {
                            alpha = items[sum][j] + alpha;
                            j--;
                        }

                        // 解析操作数
                        double factorialOperand;
                        if (!string.IsNullOrEmpty(alpha) && double.TryParse(alpha, out factorialOperand) && factorialOperand >= 0)
                        {
                            // 计算阶乘并将结果推入栈
                            double v = CalculateFactorial((int)factorialOperand);
                            data.Push(v);
                        }
                        else
                        {
                            // 输出失败的原因，以便调试
                            Console.WriteLine("Failed to parse alpha as a non-negative number.");
                        }
                    }

                    // 在 else if 中添加处理对数的逻辑
                    else if (temp == '$')
                    {
                        // 处理对数，将对数视为一个操作数
                        alpha = "";
                        while (items[sum][i + 1] != '#' && (char.IsDigit(items[sum][i + 1]) || items[sum][i + 1] == '.'))
                        {
                            alpha += items[sum][++i];
                        }

                        // 处理对数中的表达式
                        double logBase;
                        if (!string.IsNullOrEmpty(alpha) && double.TryParse(alpha, out logBase) && logBase > 0)
                        {
                            if (items[sum][i + 1] == '(')
                            {
                                // 处理括号内的表达式
                                string subExpression = "";
                                int openParentheses = 1;
                                while (openParentheses > 0 && items[sum][i + 1] != '#')
                                {
                                    char nextChar = items[sum][++i];
                                    subExpression += nextChar;
                                    if (nextChar == '(')
                                    {
                                        openParentheses++;
                                    }
                                    else if (nextChar == ')')
                                    {
                                        openParentheses--;
                                    }
                                }
                                // 计算括号内的表达式
                                double subExpressionResult = Caculate(subExpression.Split(' '), subExpression.Split(' ').Length);
                                // 将括号内的计算结果应用到对数操作
                                data.Push(Math.Log(logBase, subExpressionResult));
                            }
                            else
                            {
                                // 不涉及括号的情况，直接应用对数
                                data.Push(Math.Log(logBase, data.Pop()));
                            }
                        }
                        else
                        {
                            // alpha 不是合法的数字，或底数小于等于 0，可以在这里进行错误处理
                        }
                    }


                    else if (temp >= 'a' && temp <= 'z')
                    {
                        while (items[sum][i] != '#')
                        {
                            if (items[sum][i + 1] >= 'a' && items[sum][i + 1] <= 'z')
                            {
                                alpha += items[sum][i + 1];
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        ope.Push(alpha);
                    }
                    else if (temp == ')')
                    {
                        string now_ope;
                        while (ope.Count() != 0 && !ope.Peek().Equals("("))
                        {
                            now_ope = ope.Pop();
                            if (now_ope.Equals("+"))
                            {
                                data.Push(data.Pop() + data.Pop());
                            }
                            else if (now_ope.Equals("-"))
                            {
                                data.Push(-data.Pop() + data.Pop()); // 注意操作数顺序
                            }
                            else if (now_ope.Equals("*"))
                            {
                                data.Push(data.Pop() * data.Pop());
                            }
                            else if (now_ope.Equals("/")) // 注意操作数顺序
                            {
                                data.Push(1 / data.Pop() * data.Pop());
                            }
                            else if (now_ope.Equals("^")) // 注意操作数顺序
                            {
                                double second_num = data.Pop();
                                data.Push(Math.Pow(data.Pop(), second_num));
                            }
                            else if (now_ope.Equals("sin"))
                            {
                                data.Push(Math.Sin(data.Pop()));
                            }
                            else if (now_ope.Equals("cos"))
                            {
                                data.Push(Math.Cos(data.Pop()));
                            }
                            else if (now_ope.Equals("tan"))
                            {
                                data.Push(Math.Tan(data.Pop()));
                            }
                            else if (now_ope.Equals("abs"))
                            {
                                data.Push(Math.Abs(data.Pop()));
                            }
                            else if (now_ope.Equals("lg"))
                            {
                                data.Push(Math.Log(data.Pop()));
                            }
                            else if (now_ope.Equals("√"))
                            {
                                data.Push(Math.Sqrt(data.Pop()));
                            }
                        }
                        if (ope.Count() != 0 && ope.Peek().Equals("("))
                        {
                            ope.Pop();
                        }
                    }
                    else
                    {
                        switch (alpha)
                        {
                            case "+":
                            case "-":
                                while (ope.Count != 0 && !ope.Peek().Equals("("))
                                {
                                    string now_ope;
                                    now_ope = ope.Pop();
                                    if (now_ope.Equals("+"))
                                    {
                                        data.Push(data.Pop() + data.Pop());
                                    }
                                    else if (now_ope.Equals("-"))
                                    {
                                        data.Push(-data.Pop() + data.Pop()); // 注意操作数顺序
                                    }
                                    else if (now_ope.Equals("*"))
                                    {
                                        data.Push(data.Pop() * data.Pop());
                                    }
                                    else if (now_ope.Equals("/")) // 注意操作数顺序
                                    {
                                        data.Push(1 / data.Pop() * data.Pop());
                                    }
                                    else if (now_ope.Equals("^")) // 注意操作数顺序
                                    {
                                        double second_num = data.Pop();
                                        data.Push(Math.Pow(data.Pop(), second_num));
                                    }
                                    else if (now_ope.Equals("sin"))
                                    {
                                        data.Push(Math.Sin(data.Pop()));
                                    }
                                    else if (now_ope.Equals("cos"))
                                    {
                                        data.Push(Math.Cos(data.Pop()));
                                    }
                                    else if (now_ope.Equals("tan"))
                                    {
                                        data.Push(Math.Tan(data.Pop()));
                                    }
                                    else if (now_ope.Equals("abs"))
                                    {
                                        data.Push(Math.Abs(data.Pop()));
                                    }
                                    else if (now_ope.Equals("lg"))
                                    {
                                        data.Push(Math.Log(data.Pop()));
                                    }
                                    else if (now_ope.Equals("√"))
                                    {
                                        data.Push(Math.Sqrt(data.Pop()));
                                    }
                                }
                                ope.Push(alpha);
                                break;
                            case "*":
                            case "/":
                                while (ope.Count != 0 && ope.Peek() != "(")
                                {
                                    string now_ope;
                                    now_ope = ope.Pop();
                                    if (now_ope.Equals("*"))
                                    {
                                        data.Push(data.Pop() * data.Pop());
                                    }
                                    else if (now_ope.Equals("/")) // 注意操作数顺序
                                    {
                                        data.Push(1 / data.Pop() * data.Pop());
                                    }
                                    else if (now_ope.Equals("^")) // 注意操作数顺序
                                    {
                                        double second_num = data.Pop();
                                        data.Push(Math.Pow(data.Pop(), second_num));
                                    }
                                    else if (now_ope.Equals("sin"))
                                    {
                                        data.Push(Math.Sin(data.Pop()));
                                    }
                                    else if (now_ope.Equals("cos"))
                                    {
                                        data.Push(Math.Cos(data.Pop()));
                                    }
                                    else if (now_ope.Equals("tan"))
                                    {
                                        data.Push(Math.Tan(data.Pop()));
                                    }
                                    else if (now_ope.Equals("abs"))
                                    {
                                        data.Push(Math.Abs(data.Pop()));
                                    }
                                    else if (now_ope.Equals("lg"))
                                    {
                                        data.Push(Math.Log(data.Pop()));
                                    }
                                    else if (now_ope.Equals("√"))
                                    {
                                        data.Push(Math.Sqrt(data.Pop()));
                                    }
                                }
                                ope.Push(alpha);
                                break;
                            case "^":
                                ope.Push(alpha);
                                break;
                            case "(":
                                ope.Push(alpha);
                                break;
                        }
                    }
                    alpha = "";
                }
                while (ope.Count != 0)
                {
                    alpha = ope.Pop();
                    if (alpha.Equals("+"))
                    {
                        data.Push(data.Pop() + data.Pop());
                    }
                    else if (alpha.Equals("-"))
                    {
                        data.Push(-data.Pop() + data.Pop());
                    }
                    else if (alpha.Equals("*"))
                    {
                        data.Push(-data.Pop() * data.Pop());
                    }
                    else if (alpha.Equals("/"))
                    {
                        data.Push(1 / data.Pop() * data.Pop());
                    }
                    else if (alpha.Equals("^"))
                    {
                        double second_num = data.Pop();
                        data.Push(Math.Pow(data.Pop(), second_num));
                    }
                    else if (alpha.Equals("sin"))
                    {
                        data.Push(Math.Sin(data.Pop()));
                    }
                    else if (alpha.Equals("cos"))
                    {
                        data.Push(Math.Cos(data.Pop()));
                    }
                    else if (alpha.Equals("tan"))
                    {
                        data.Push(Math.Tan(data.Pop()));
                    }
                    else if (alpha.Equals("abs"))
                    {
                        data.Push(Math.Abs(data.Pop()));
                    }
                    else if (alpha.Equals("lg"))
                    {
                        data.Push(Math.Log(data.Pop()));
                    }
                    else if (alpha.Equals("√"))
                    {
                        data.Push(Math.Sqrt(data.Pop()));
                    }
                }
                result += data.Peek();
            }
            return result;
        }

        private void Button_0_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "0";
        }

        private void Button_1_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "1";
        }

        private void Button_2_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "2";
        }

        private void Button_3_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "3";
        }

        private void Button_4_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "4";
        }

        private void Button_5_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "5";
        }

        private void Button_6_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "6";
        }

        private void Button_7_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "7";
        }

        private void Button_8_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "8";
        }

        private void Button_9_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "9";
        }

        private void Button_dot_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += ".";
        }

        private void Button_ln_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "ln";
        }

        private void Button_back_Click(object sender, RoutedEventArgs e)
        {
            // 检查文本中是否以要删除的字符串结尾
            foreach (var symbol in symbolsToRemove)
            {
                if (textBox.Text.EndsWith(symbol))
                {
                    // 如果是，则一次性删除整个字符串
                    textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - symbol.Length);
                    return;
                }
            }

            // 如果文本中没有要删除的字符串，则删除最后一个字符
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
            }
        }

        private void Button_equal_Click(object sender, RoutedEventArgs e)
        {
            record = textBox.Text;
            item_divide(textBox.Text, out string[] items, out int sum);
            double result = Caculate(items, sum);
            historyListBox.Items.Add($"{record}={result}");
            textBox.Text = Convert.ToString(result);

        }

        private void Button_sub_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "-";
        }

        private void Button_times_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "*";
        }

        private void Button_clear_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = "";
        }

        private void Button_abs_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "abs";
        }

        private void Button_tan_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "tan";
        }

        private void Button_cos_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "cos";
        }

        private void Button_sin_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "sin";
        }

        private void Button_log_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "log";
        }

        private void Button_sqrt_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "√";
        }

        private void Button_power_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "^";
        }

        private void Button_remainder_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "%";
        }

        private void Button_factorial_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "!";
        }

        private void Button_RightBracket_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += ")";
        }

        private void Button_LeftBracket_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "(";
        }

        private void Button_plus_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "+";
        }

        private void Button_division_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text += "/";
        }

        // 计算阶乘的静态方法
        private static int CalculateFactorial(int n)
        {
            if (n == 0 || n == 1)
            {
                return 1;
            }
            else
            {
                return n * CalculateFactorial(n - 1);
            }
        }


        
        private void Button_pi_Click(object sender, RoutedEventArgs e)
        {
            textbox.Text += "π";
        }
    }
}
