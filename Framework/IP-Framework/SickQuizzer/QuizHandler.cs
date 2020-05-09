﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Quizzer
{
    public class QuizHandler
    {
        private Dictionary<long, Quiz> quizes;

        public QuizHandler()
        {
            quizes = new Dictionary<long, Quiz>();
        }

        public Question GetQuestion(long id)
        {
            if (quizes.ContainsKey(id))
            {
                return quizes[id].GetNextQuestion();
            }
            else
            {
                QuSymptomsParser symptomsParser = new QuSymptomsParser();

                symptomsParser.FeedSignatures("D:\\diseases");

                ISet<QuSignature> signatures = symptomsParser.GetSignatures();

                SymptomsHolder symptomsHolder = new SymptomsHolder(signatures);

                Quiz quiz = new Quiz(symptomsHolder);

                quizes.Add(id, quiz);
                quiz.BeginQuiz();

                return quiz.GetNextQuestion();
            }
        }

        public bool ProcessAnswer(long id, Answer answer)
        {
            if (quizes.ContainsKey(id))
            {
                return quizes[id].ProcessAnswer(answer);
            }
            return false;
        }
    }

}
