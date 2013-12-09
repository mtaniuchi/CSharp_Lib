using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReplaceText
{
    /// <summary>
    /// ReplaceText
    /// </summary>
    /// <seealso cref="http://thenounproject.com/noun/text/#icon-No8326">Icon Text designed by Henrik LM from The Noun Project</seealso>
    /// <seealso cref="http://thenounproject.com/HenrikLundMikkelsen"/>
    /// <remarks>文字列を置換するプログラムです</remarks>
    public class Program
    {
        /// <summary>成功時のリターンコード</summary>
        private const int EXIT_SUCCESS = 0;
        /// <summary>失敗時のリターンコード</summary>
        private const int EXIT_FAILURE = -1;

        /// <summary>
        /// コンソールアプリケーションメイン
        /// </summary>
        /// <param name="args">
        /// <para>引数1:対象ファイル</para>
        /// <para>引数2:コンフィグファイル</para>
        /// </param>
        /// <returns>
        /// <para>結果コード</para>
        /// <para>（成功=0, 失敗=-1）</para></returns>
        /// <remarks>
        /// <para>勢いで作ったので改善点沢山あります。</para>
        /// <para>TODO:コンフィグファイルが無くてもすぐに置換できるモードが欲しい</para>
        /// <para>TODO:コンフィグファイルのフォーマットをチェックする</para>
        /// </remarks>
        public static int Main(string[] args)
        {
            if (args.Count() != 2)
            {
                Console.WriteLine(@"USAGE: ReplaceText.exe TargetFilePath ConfigFilePath");
                Console.WriteLine(@"\tTargetFilePath:text file");
                Console.WriteLine(@"\tConfigFilePath:text file with Tabbed key value pairs");
                return EXIT_FAILURE;
            }

            var targetFileName = args[0];
            var configFileName = args[1];

            // get encode
            var targetFileEnc = FileUtility.GetCode(targetFileName);
            var configFileEnc = FileUtility.GetCode(configFileName);

            Console.WriteLine("\tTargetFile encoding:" + targetFileEnc.EncodingName);
            Console.WriteLine("\tConfigFile encoding:" + configFileEnc.EncodingName);

            // read config file
            var replacePair = new Dictionary<string, string>();
            try
            {
                using (var sr = new StreamReader(configFileName, configFileEnc))
                {
                    string line;
                    long lineNumber = 1;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // make key-value pair for replacement
                        var pair = line.Split('\t');

                        if (pair.Count() != 2)
                        {
                            lineNumber++;
                            continue;
                        }

                        string value;
                        if (replacePair.TryGetValue(pair[0], out value))
                        {
                            Console.WriteLine(@"key is duplicated in line {0}.", lineNumber);
                            return EXIT_FAILURE;
                        }

                        replacePair.Add(pair[0], pair[1]);

                        lineNumber++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return EXIT_FAILURE;
            }

            // read target file.
            string targetText;
            try
            {
                using (var sr = new StreamReader(targetFileName, targetFileEnc))
                {
                    targetText = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return EXIT_FAILURE;
            }

            // replace!
            try
            {
                targetText = replacePair.Aggregate(targetText, (current, pair) => current.Replace(pair.Key, pair.Value));

                File.WriteAllText(targetFileName, targetText, targetFileEnc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return EXIT_FAILURE;
            }

            Console.WriteLine("\tReplaced:{0}", targetFileName);
            return EXIT_SUCCESS;
        }
    }
}
