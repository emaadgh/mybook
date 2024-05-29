using MyBook.API.Helpers;

namespace MyBook.Test.Helpers
{
    public class PagedListTests
    {
        PagedList<int> pagedList;

        public PagedListTests()
        {
            pagedList = new PagedList<int>(new List<int> { 1, 2, 3 }, 3, 2, 1);
        }

        [Fact]
        public void PagedList_HasPrevious_HasPreviousMustReturnTrue()
        {
            //Act
            bool hasPrevious = pagedList.HasPrevious;

            //Assert
            Assert.True(hasPrevious);
        }

        [Fact]
        public void PagedList_HasNext_HasNextMustReturnTrue()
        {
            //Act
            bool hasNext = pagedList.HasNext;

            //Assert
            Assert.True(hasNext);
        }
    }
}
