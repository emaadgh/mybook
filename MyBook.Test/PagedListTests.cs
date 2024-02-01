using MyBook.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBook.Test
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
            //Arange 

            //Act
            bool hasPrevious = pagedList.HasPrevious;

            //Assert
            Assert.True(hasPrevious);
        }

        [Fact]
        public void PagedList_HasNext_HasNextMustReturnTrue()
        {
            //Arange 

            //Act
            bool hasNext = pagedList.HasNext;

            //Assert
            Assert.True(hasNext);
        }
    }
}
