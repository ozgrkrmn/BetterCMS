﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;

using BetterCms.Core.DataAccess;
using BetterCms.Core.DataAccess.DataContext;
using BetterCms.Core.DataContracts;
using BetterCms.Core.Exceptions.DataTier;
using BetterCms.Core.Models;
using BetterCms.Core.Security;
using BetterCms.Module.Root.Models;
using BetterCms.Module.Root.Mvc;

using FluentNHibernate.Utils;

using NHibernate.Criterion;
using NHibernate.Linq;

namespace BetterCms.Module.Root.Services
{
    internal class DefaultCategoryService : ICategoryService
    {
        /// <summary>
        /// The repository
        /// </summary>
        private readonly IRepository repository;

        /// <summary>
        /// The CMS configuration.
        /// </summary>
        private readonly ICmsConfiguration cmsConfiguration;

        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCategoryService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="cmsConfiguration">The CMS configuration.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public DefaultCategoryService(IRepository repository, ICmsConfiguration cmsConfiguration, IUnitOfWork unitOfWork)
        {
            this.repository = repository;
            this.cmsConfiguration = cmsConfiguration;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Gets the list of category lookup values.
        /// </summary>
        /// <returns>
        /// List of category lookup values
        /// </returns>
        public IEnumerable<LookupKeyValue> GetCategories()
        {
            return repository
                .AsQueryable<Category>()
                .Select(c => new LookupKeyValue
                {
                    Key = c.Id.ToString().ToLowerInvariant(),
                    Value = c.Name
                })
                .ToFuture();
        }

        public IEnumerable<Guid> GetSelectedCategoriesIds<TEntity>(Guid? entityId) where TEntity : Entity, ICategorized
        {
            TEntity entityAlias = null;
            Category categoryAlias = null;

            return repository.AsQueryOver<Category>()
                             .WithSubquery.WhereProperty(c => c.Id)
                             .In(QueryOver.Of(() => entityAlias)
                                     .JoinQueryOver(() => entityAlias.Categories, () => categoryAlias)
                                     .Where(() => entityAlias.Id == entityId)
                                     .SelectList(list => list.Select(() => categoryAlias.Id)))
                                     .Select(category => category.Id)
                                     .Future<Guid>();
        }

        public Category SaveCategory(out bool categoryUpdated, CategoryTree categoryTree, Guid categoryId, int version, string name, int displayOrder, string macro, Guid parentCategoryId, bool isDeleted = false, Category parentCategory = null, List<Category> categories = null)
        {
            categoryUpdated = false;

            var category = categoryId.HasDefaultValue()
                ? new Category()
                : categories != null ? categories.First(c => c.Id == categoryId) : repository.First<Category>(categoryId);

            if (isDeleted && !category.Id.HasDefaultValue())
            {
                repository.Delete(category);
                categoryUpdated = true;
            }
            else
            {
                var updated = false;
                if (category.CategoryTree == null)
                {
                    category.CategoryTree = categoryTree;
                }

                if (category.Name != name)
                {
                    updated = true;
                    category.Name = name;
                }

                if (category.DisplayOrder != displayOrder)
                {
                    updated = true;
                    category.DisplayOrder = displayOrder;
                }

                Category newParent;
                if (parentCategory != null && !parentCategory.Id.HasDefaultValue())
                {
                    newParent = parentCategory;
                }
                else
                {
                    newParent = parentCategoryId.HasDefaultValue()
                        ? null
                        : repository.AsProxy<Category>(parentCategoryId);
                }

                if (category.ParentCategory != newParent)
                {
                    updated = true;
                    category.ParentCategory = newParent;
                }

                if (cmsConfiguration.EnableMacros && category.Macro != macro)
                {
                    category.Macro = macro;
                    updated = true;
                }

                if (updated)
                {
                    category.Version = version;
                    repository.Save(category);
                    categoryUpdated = true;
                }
            }

            return category;
        }

        public void DeleteCategoryTree(Guid id, int version, IPrincipal currentUser)
        {
            var categoryTree = repository
                .AsQueryable<CategoryTree>()
                .Where(map => map.Id == id)
                // TODO:                .FetchMany(map => map.AccessRules)
                .Distinct()
                .ToList()
                .First();

            // TODO:            // Security.
            //            if (cmsConfiguration.Security.AccessControlEnabled)
            //            {
            //                var roles = new[] { RootModuleConstants.UserRoles.EditContent };
            //                accessControlService.DemandAccess(sitemap, currentUser, AccessLevel.ReadWrite, roles);
            //            }

            // Concurrency.
            if (version > 0 && categoryTree.Version != version)
            {
                throw new ConcurrentDataException(categoryTree);
            }

            unitOfWork.BeginTransaction();

            // TODO:
            //            if (sitemap.AccessRules != null)
            //            {
            //                var rules = sitemap.AccessRules.ToList();
            //                rules.ForEach(sitemap.RemoveRule);
            //            }

            repository.Delete(categoryTree);

            unitOfWork.Commit();

            // Events.
            // TODO:            Events.SitemapEvents.Instance.OnSitemapDeleted(categoryTree);
        }

        public void CombineEntityCategories<TEntity>(TEntity entity, IEnumerable<System.Guid> currentCategories) where TEntity : Entity, ICategorized
        {
            var categories = currentCategories != null ? currentCategories.ToList() : new List<Guid>();

            if (entity != null)
            {
                var newCategoryIds = entity.Categories != null ? categories.Where(cId => entity.Categories.All(pc => pc.Id != cId)).ToArray() : categories.ToArray();
                var newCategories = repository.AsQueryOver<Category>().WhereRestrictionOn(t => t.Id).IsIn(newCategoryIds).Future<Category>();

                if (entity.Categories != null)
                {
                    // Remove categories
                    var removedCategories = entity.Categories.Where(c => !categories.Contains(c.Id)).ToList();

                    foreach (var removedCategory in removedCategories)
                    {
                        entity.RemoveCategory(removedCategory);
                    }
                }

                // Attach new categories
                foreach (var newCategory in newCategories)
                {
                    entity.AddCategory(newCategory);
                }              
            }
        }
    }
}