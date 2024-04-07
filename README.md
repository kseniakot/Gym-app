# Котова Ксения Алексеевна, группа 253503

## Мобильное приложение для сети тренажерных залов

### Основные классы и их функции:

#### 1. **IUser**

*поля:*
- string login(phone number)
- string password

*методы:*
- ChangePassword() - поменять пароль


#### 2. **Personal_info**

*поля:*
- string name
- string surname
- DateTime date_of_birth
- Location address

*методы:*
- EditName/Surname/DateOfBirth/Address() - функции для редактирования


#### 3. **Location**

*поля:*
- string city
- string street
- int building

*методы:*
- EditСity/Street/Building() - функции редактирования


#### 4. **Admin** : IUser

*поля:*
- Gym gym

*методы:*
- ListGymClients/Treners/Members/Memberships() - вывести списки зала
- RemoveClient/Trener/Member/Membership() - удалить кого-то из списка
- AddTrener/Membership()- функции чтоб добавить тренера/абонемент
- СalculateTrenerSalary() - рассчитать зарплату для тренера
- SearchTrener/Member/Client() - функции поиска
- SortTreners/Members() - функции сортировки
- ShowTrenerClients() - выводит список клиентов тренера
- Show/Add/RemoveExercise() - функции вывода, добавления или удаления упражнений



#### 5. **Gym**

*поля:*
- string name
- Location address
- List<Trener> Treners
- List<Client> Clients
- List<Membership> Memberships

*методы:*
- ListGymClients/Treners/Members/Memberships() - вывести списки 
- RemoveGymClients/Treners/Members/Memberships() - функции удаления
- AddGymTrener/Membership() - функции добавления
- SearchTrener/Client/Member() - функции поиска
- SortTreners/Members() - функции для сортировки


#### 6. **Trener** : User

*поля:*
- PersonalInfo personalInfo
- int salary
- int price
- List<Member> MyClients

*методы:*
- ShowWorkoutPrice()
- ListMyClients()
- Create/Show/EditClientWorkout() - функции для создания тренировок клиентам
- Сreate/EditClientNutritionPlan() - функции для создания плана питания клиента


#### 7. **Client** : User

*поля:*
- PersonalInfo personalInfo
- ShoppingCart shoppingCart

*методы:*
- EditName/Surname/DateOfBirth/Address()
- AddMembershipToCart() - добавить в корзину
- RemoveMembershipFromCart() - убрать из корзины
- CheckoutShoppingCart() - оплатить корзину


#### 8. **Shopping_cart:**

*поля:*
- List<Membership>

*методы:*
- Add/RemoveMembership() - добавить/убрать абонемент из корзины
- CalculateTotalAmount() - посчитать полную сумму
- Checkout() - купить 


#### 9. **Member** : Client

*поля:*
- List<Membership> my_memberships
- Nutrition nutrition
- Calendar calendar
- List<Workout> my_workouts

*методы:*
- ListMyMemberships() - вывести список моих абонементов
- ShowCurrentMembershipInfo()-показывает дату, до которой абонемент активен
- Activate/FreezeMembership() - активация/заморозка
- Add/Remove/EditWorkout()
- show_set/edit_nutrition_plan()
- show_set_edit_training_days()


#### 10. **Workout**

*поля:*
- string name
- List<Excercise> excersises

*методы:*
- Add/RemoveExercise()


#### 11. **Exercise**

*поля:*
- string name
- int num_of_sets
- int num_of_reps
- Dictionary<set:<reps, weight>>

*методы:*
- Set/EditReps/Weight()
- ShowReps/Weight()
- Add/Remove/EditSet()


#### 12. **Membership**

*поля:*
- DateTime start
- DateTime end
- DateTime price
- string TrainerName(optional)
- bool isActive

*методы:*
- ShowActiveUntil() - выводит дату, в которую закончится срок действия
- HasTrener() - покажет есть тренер или нет
- IsActive() - покажет активен ли абонемент
- Activate() - активировать
- Freeze() - заморозить


#### 13. **Nutrition**

*поля:*
- protein
- fats
- carbs

*методы:*
- СalculateСalories() - считает калории на основе введенных белков, жиров и углеводов
- EditProtein/Fats/Carbs/Calories()
- CalculateMacronutrients() - считает бжу на основе данных о росте и весе


#### 14. **Calendar**:

*поля:*
- Dictionary<DayOfWeek, Workout> schedule
- Dictionary<DayOfWeek, NutritionLog> nutritionLog

*методы:*
- ShowWorkout()
- PlanWorkout()
- CancelWorkout()
- LogNutrition() - внести текущее значение по питанию за день
- Show/EditNutrition() - просмотреть/отредактировать текущее питание


  ![gym_system](https://github.com/kseniakot/Gym-app/assets/113253599/e5fde64f-81e9-4eee-ae10-859aba4b6e6a)


