namespace VoiceConcierge.Infrastructure.Data.Seed;

/// <summary>
/// Seed data for The Meridian Casino & Resort
/// Based on the Property Requirements Document
/// </summary>
public static class MeridianSeedData
{
    public static class VoiceConfigurations
    {
        public static readonly List<(int VoiceId, string Name, string Description, string Gender, string Accent, string ProviderVoiceId)> Voices = new()
        {
            (
                VoiceId: 1,
                Name: "James",
                Description: "Mature, warm British accent - professional and refined. Perfect for luxury hospitality.",
                Gender: "Male",
                Accent: "British",
                ProviderVoiceId: "onyx" // OpenAI TTS voice
            ),
            (
                VoiceId: 2,
                Name: "Sofia",
                Description: "Friendly, subtle European accent - welcoming and elegant. Sophisticated and approachable.",
                Gender: "Female",
                Accent: "European",
                ProviderVoiceId: "nova" // OpenAI TTS voice
            ),
            (
                VoiceId: 3,
                Name: "Marcus",
                Description: "American, confident and energetic - modern and approachable. Warm and enthusiastic.",
                Gender: "Male",
                Accent: "American",
                ProviderVoiceId: "echo" // OpenAI TTS voice
            ),
            (
                VoiceId: 4,
                Name: "Elena",
                Description: "American, calm and reassuring - sophisticated and clear. Polished and professional.",
                Gender: "Female",
                Accent: "American",
                ProviderVoiceId: "shimmer" // OpenAI TTS voice
            )
        };
    }

    public static class FAQs
    {
        public static readonly List<(string Category, string Question, string Answer)> Items = new()
        {
            // Gaming - Poker Room
            (
                Category: "Gaming",
                Question: "What are the poker room hours?",
                Answer: "Our poker room is open 24 hours a day, 7 days a week. We offer Texas Hold'em, Omaha, and Seven Card Stud with various limits to suit all players."
            ),
            (
                Category: "Gaming",
                Question: "Is the poker room open right now?",
                Answer: "Yes! Our poker room is open 24/7, so you can play anytime. We have tables running around the clock."
            ),
            (
                Category: "Gaming",
                Question: "What poker games do you offer?",
                Answer: "We offer Texas Hold'em, Omaha, and Seven Card Stud. Stakes range from $1/$2 no-limit to $25/$50 limit games. We also host daily tournaments."
            ),
            (
                Category: "Gaming",
                Question: "Are there poker tournaments?",
                Answer: "Yes! We have daily tournaments at 11 AM and 7 PM with buy-ins starting at $200. Special weekend tournaments feature larger prize pools. Check with the poker room for the current schedule."
            ),
            
            // Gaming - Casino Floor
            (
                Category: "Gaming",
                Question: "What time does the casino open?",
                Answer: "The Meridian Casino is open 24 hours a day, every day of the year. You can enjoy slots, table games, and poker at any time."
            ),
            (
                Category: "Gaming",
                Question: "When can I gamble?",
                Answer: "Our casino floor is open 24/7. You're welcome to play slots, table games, and poker at any time that suits you."
            ),
            (
                Category: "Gaming",
                Question: "What table games are available?",
                Answer: "We offer Blackjack, Roulette, Craps, Baccarat, and Pai Gow Poker. Table minimums range from $10 to $100 depending on the time of day."
            ),
            (
                Category: "Gaming",
                Question: "Do you have slot machines?",
                Answer: "Yes! We have over 1,500 slot machines ranging from penny slots to high-limit machines. We offer both classic and the latest video slots."
            ),
            
            // Dining - Eclipse Lounge
            (
                Category: "Dining",
                Question: "What are Eclipse Lounge hours?",
                Answer: "Eclipse Lounge is open daily from 5 PM to 2 AM. We're located on the 32nd floor with stunning views of the Las Vegas Strip."
            ),
            (
                Category: "Dining",
                Question: "Tell me about Eclipse Lounge",
                Answer: "Eclipse Lounge is our signature rooftop venue on the 32nd floor. We serve craft cocktails, premium spirits, and small plates with breathtaking Strip views. Open 5 PM to 2 AM daily. Reservations recommended for groups larger than 4."
            ),
            (
                Category: "Dining",
                Question: "Can I make a reservation at Eclipse Lounge?",
                Answer: "Yes! We recommend reservations, especially for groups of 4 or more. You can book by calling our concierge desk at extension 0 or through our front desk."
            ),
            (
                Category: "Dining",
                Question: "Does Eclipse Lounge have a dress code?",
                Answer: "Eclipse Lounge has a smart casual dress code. We ask that guests avoid athletic wear, flip-flops, and torn clothing. Business casual or evening wear is perfect."
            ),
            
            // Dining - Restaurants & Partners
            (
                Category: "Dining",
                Question: "What restaurants are nearby?",
                Answer: "We have excellent dining partners within 10 minutes of The Meridian. Partner restaurants offer our guests a 15% discount. Ask our concierge for the current list and to make reservations."
            ),
            (
                Category: "Dining",
                Question: "Are there good restaurants near the hotel?",
                Answer: "Absolutely! We've partnered with several exceptional restaurants within a 10-minute walk. Our guests receive 15% off at all partner locations. Visit our concierge desk for recommendations and reservations."
            ),
            (
                Category: "Dining",
                Question: "Do you offer discounts at restaurants?",
                Answer: "Yes! We offer 15% off at our partner restaurants located within 10 minutes of the property. Our concierge can provide you with the full list and help with reservations."
            ),
            
            // Accommodations
            (
                Category: "Accommodations",
                Question: "What types of rooms do you have?",
                Answer: "We offer Deluxe Rooms, Premium Suites, and Luxury Penthouses. All rooms feature premium bedding, marble bathrooms, and modern amenities. Many rooms offer stunning Strip views."
            ),
            (
                Category: "Accommodations",
                Question: "What amenities are in the rooms?",
                Answer: "All rooms include premium bedding, marble bathrooms, 55-inch smart TVs, high-speed WiFi, mini-bar, coffee maker, safe, and luxury bath products. Suites add separate living areas and some feature private balconies."
            ),
            (
                Category: "Accommodations",
                Question: "Can I get a room upgrade?",
                Answer: "Room upgrades are subject to availability. We offer complimentary upgrades for loyalty members when available, or you can request a paid upgrade at check-in. Celebration packages also include automatic upgrades."
            ),
            (
                Category: "Accommodations",
                Question: "Is there WiFi in the rooms?",
                Answer: "Yes, all rooms include complimentary high-speed WiFi. Premium bandwidth is also available for an additional fee if you need it for work or streaming."
            ),
            
            // Services - Concierge
            (
                Category: "Services",
                Question: "How can I reach the concierge?",
                Answer: "You can reach our concierge desk by dialing extension 0 from any hotel phone, visiting our lobby desk, or speaking with me - I'm here to help 24/7!"
            ),
            (
                Category: "Services",
                Question: "What can the concierge help with?",
                Answer: "Our concierge can assist with restaurant reservations, show tickets, transportation, tour bookings, special requests, celebration planning, and recommendations for Las Vegas attractions."
            ),
            
            // Services - Celebration Packages
            (
                Category: "Services",
                Question: "I want to propose to my girlfriend",
                Answer: "How exciting! We have several celebration packages perfect for proposals. Packages start at $500 and include room upgrades, champagne, dinner credits, and rose petals. Eclipse Lounge also offers private terrace reservations with stunning Strip views. I'd recommend speaking with our concierge desk to plan the perfect moment - shall I give you their direct number?"
            ),
            (
                Category: "Services",
                Question: "Do you have celebration packages?",
                Answer: "Yes! Our celebration packages start at $500 and include room upgrades, champagne, dinner credits, and special amenities like rose petals or balloons. Perfect for proposals, anniversaries, or birthdays. Contact our concierge to customize your package."
            ),
            (
                Category: "Services",
                Question: "How much are celebration packages?",
                Answer: "Celebration packages start at $500 and can be customized based on your needs. They typically include a room upgrade, champagne, dinner credit, and special touches. Our concierge can help create the perfect package for your occasion."
            ),
            
            // Check-in/Check-out
            (
                Category: "Services",
                Question: "What time is check-in?",
                Answer: "Check-in time is 3 PM and check-out is 12 PM noon. Early check-in and late check-out are available upon request based on availability. Please contact the front desk to arrange special times."
            ),
            (
                Category: "Services",
                Question: "What time is check-out?",
                Answer: "Check-out time is 12 PM noon. Late check-out is available upon request, subject to availability. Please contact the front desk if you need to extend your stay."
            ),
            (
                Category: "Services",
                Question: "Can I get early check-in?",
                Answer: "Early check-in is available based on room availability. We'll do our best to accommodate you! Please contact the front desk upon arrival to check if your room is ready."
            ),
            
            // Parking & Transportation
            (
                Category: "Services",
                Question: "Is there parking available?",
                Answer: "Yes, we offer both self-parking and valet parking. Self-parking is complimentary for guests. Valet parking is available for $35 per day and includes in-and-out privileges."
            ),
            (
                Category: "Services",
                Question: "How much is valet parking?",
                Answer: "Valet parking is $35 per day with unlimited in-and-out privileges. Self-parking is complimentary for all guests. The valet stand is located at the main entrance."
            ),
            (
                Category: "Services",
                Question: "Do you offer airport transportation?",
                Answer: "We can arrange airport transportation through our concierge. Options include hotel shuttle service, luxury car service, or we can help you book a rideshare. Contact our concierge for pricing and availability."
            ),
            
            // Pool & Spa
            (
                Category: "Amenities",
                Question: "Do you have a pool?",
                Answer: "Yes! Our rooftop pool deck is open daily from 8 AM to 8 PM, offering cabanas, a hot tub, and poolside service. The pool is heated year-round."
            ),
            (
                Category: "Amenities",
                Question: "What are the pool hours?",
                Answer: "Our rooftop pool is open daily from 8 AM to 8 PM. We offer cabana rentals, poolside drink service, and a heated pool and hot tub."
            ),
            (
                Category: "Amenities",
                Question: "Can I rent a cabana?",
                Answer: "Yes! Pool cabanas are available for rent and include shaded seating, fresh towels, bottled water, and dedicated server service. Prices start at $150 per day. Reserve through our concierge."
            ),
            (
                Category: "Amenities",
                Question: "Do you have a spa?",
                Answer: "Yes, our Serenity Spa offers massages, facials, body treatments, and a relaxation lounge. We're open daily 9 AM to 9 PM. Book through our concierge or call extension 777."
            ),
            
            // Fitness Center
            (
                Category: "Amenities",
                Question: "Is there a gym?",
                Answer: "Yes! Our 24-hour fitness center features cardio equipment, free weights, strength training machines, and complimentary towels and water. Located on the 3rd floor."
            ),
            (
                Category: "Amenities",
                Question: "What are gym hours?",
                Answer: "Our fitness center is open 24 hours a day for guest convenience. We have cardio equipment, free weights, and strength machines, plus complimentary towels and water."
            ),
            
            // Shows & Entertainment
            (
                Category: "Entertainment",
                Question: "Can you help me get show tickets?",
                Answer: "Absolutely! Our concierge can help you book tickets to any Las Vegas show. We have access to all major productions, Cirque du Soleil, concerts, and more. Just let us know what you're interested in!"
            ),
            (
                Category: "Entertainment",
                Question: "What shows do you recommend?",
                Answer: "Las Vegas has incredible shows! Popular options include Cirque du Soleil productions, magic shows, comedy acts, and concerts. Our concierge stays current on all entertainment and can recommend shows based on your interests and help with bookings."
            ),
            
            // General Information
            (
                Category: "General",
                Question: "What is your address?",
                Answer: "The Meridian Casino & Resort is located at 3799 Las Vegas Boulevard South, Las Vegas, Nevada 89109. We're on the Strip between Flamingo and Tropicana."
            ),
            (
                Category: "General",
                Question: "What is your phone number?",
                Answer: "Our main number is (702) 555-MERIDIAN. For reservations, call (888) 555-RESORT. From your room, dial 0 for the front desk or concierge."
            ),
            (
                Category: "General",
                Question: "Do you allow pets?",
                Answer: "We welcome dogs up to 50 pounds with a $150 non-refundable pet fee. Service animals are always welcome at no charge. Please notify us at booking if traveling with a pet."
            ),
            (
                Category: "General",
                Question: "Is there a resort fee?",
                Answer: "Yes, we have a daily resort fee of $45 plus tax per room. This includes WiFi, fitness center access, pool access, local calls, and printing services at the business center."
            ),
            (
                Category: "General",
                Question: "Do you have smoking rooms?",
                Answer: "We offer both smoking and non-smoking rooms. Please specify your preference at booking. Smoking is also permitted in designated areas of the casino floor."
            )
        };
    }
}
