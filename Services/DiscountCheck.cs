namespace FourtitudeTest.Services
{
    public class DiscountCheck
    {

        public (long totalDiscount, long finalAmount) CalculateDiscount(long totalAmount)
        {
            // Step 1: Calculate Base Discount
            double baseDiscountPercentage = GetBaseDiscountPercentage(totalAmount);
            long baseDiscount = (long)(totalAmount * baseDiscountPercentage);

            // Step 2: Calculate Conditional Discounts
            double conditionalDiscountPercentage = GetConditionalDiscountPercentage(totalAmount);
            long conditionalDiscount = (long)(totalAmount * conditionalDiscountPercentage);

            // Step 3: Total Discount
            long totalDiscount = baseDiscount + conditionalDiscount;

            // Step 4: Cap on Maximum Discount (20% of totalAmount)
            long maxDiscountAllowed = (long)(totalAmount * 0.20);
            if (totalDiscount > maxDiscountAllowed)
            {
                totalDiscount = maxDiscountAllowed;
            }

            // Step 5: Final Amount
            long finalAmount = totalAmount - totalDiscount;

            return (totalDiscount, finalAmount);
        }

        private double GetBaseDiscountPercentage(long totalAmount)
        {
            if (totalAmount < 20000)
                return 0; // No discount
            if (totalAmount >= 20000 && totalAmount <= 50000)
                return 0.05; // 5% discount
            if (totalAmount >= 50100 && totalAmount <= 80000)
                return 0.07; // 7% discount
            if (totalAmount >= 80100 && totalAmount <= 120000)
                return 0.10; // 10% discount
            return 0.15; // 15% discount for amounts > 1200
        }

        private double GetConditionalDiscountPercentage(long totalAmount)
        {
            double conditionalDiscount = 0;

            // Prime number condition
            if (totalAmount > 50000 && IsPrime(totalAmount))
            {
                conditionalDiscount += 0.08; // Additional 8% discount for prime numbers above 500
            }

            // Ends in 5 condition
            if (totalAmount > 90000 && totalAmount % 10 == 5)
            {
                conditionalDiscount += 0.10; // Additional 10% discount for amounts > 900 that end in 5
            }

            return conditionalDiscount;
        }
            private bool IsPrime(long number)
            {
                if (number <= 1) return false;
                if (number == 2) return true;
                if (number % 2 == 0) return false;

                for (int i = 3; i <= number / 2; i += 2)
                {
                    if (number % i == 0) return false;
                }

                return true;
            }
        }
}