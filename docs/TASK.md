```pascal
procedure CalcPrices(InputPriceWithNDS: double; ProcNDS: TProcNDS; out CorrectedPriceWithNDS, CorrectedPriceWithoutNDS: double);
begin
//  Есть правило:
//  [Цена с НДС] = [Цена без НДС] * (1 + [Процент НДС] / 100)
//  Или запишем иначе
//  CorrectedPriceWithNDS = CorrectedPriceWithoutNDS * (1 + ProcNDS / 100)

//  Входные параметры процедуры:
//  1. Внешняя система рассчитывает и передает нам рекомендованную цену с НДС - InputPriceWithNDS,
//  содержащую до 20 знаков после зарпятой
//  2. Процент НДС  -  ProcNDS : 0..99

//  ЗАДАЧА: Процедура должна рассчитать значение цен с НДС и без НДС (CorrectedPriceWithNDS,
//  CorrectedPriceWithoutNDS) так, чтобы CorrectedPriceWithNDS было выполнено условие
//  InputPriceWithNDS: |CorrectedPriceWithNDS-InputPriceWithNDS| -> min ...  и при этом
//  CorrectedPriceWithNDS и CorrectedPriceWithoutNDS содержали максимум 2 знака после
//  запятой и не требовали округлений.

//  3. Сделать пользовательский интерфейс для проверки корректности вашего решения, консольный тоже ок. Используем только стандартные библиотеки.
//  4. Прислать в архиве на почту все файлы проекта. В случае проблем с отправкой установить пароль 321.
end;

end.
```