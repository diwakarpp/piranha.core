﻿/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Piranha.Services;
using Piranha.Repositories;
using System;

namespace Piranha
{
    /// <summary>
    /// The main application api.
    /// </summary>
    public sealed class Api : IApi, IDisposable
    {
        /// <summary>
        /// The private model cache.
        /// </summary>
        private readonly ICache _cache;

        /// <summary>
        /// Gets/sets the alias service.
        /// </summary>
        public IAliasService Aliases { get; }

        /// <summary>
        /// Gets/sets the archive service.
        /// </summary>
        public IArchiveService Archives { get; }

        /// <summary>
        /// Gets/sets the content service.
        /// </summary>
        public IContentService Content { get; set; }

        /// <summary>
        /// Gets/sets the content group repository.
        /// </summary>
        public IContentGroupService ContentGroups { get; set; }

        /// <summary>
        /// Gets/sets the content type service.
        /// </summary>
        public IContentTypeService ContentTypes { get; set; }

        /// <summary>
        /// Gets/sets the language service.
        /// </summary>
        public ILanguageService Languages { get; set; }

        /// <summary>
        /// Gets the media service.
        /// </summary>
        /// <returns></returns>
        public IMediaService Media { get; }

        /// <summary>
        /// Gets the page service.
        /// </summary>
        public IPageService Pages { get; }

        /// <summary>
        /// Gets the page type service.
        /// </summary>
        public IPageTypeService PageTypes { get; }

        /// <summary>
        /// Gets the param service.
        /// </summary>
        public IParamService Params { get; }

        /// <summary>
        /// Gets the post service.
        /// </summary>
        public IPostService Posts { get; }

        /// <summary>
        /// Gets the post type service.
        /// </summary>
        public IPostTypeService PostTypes { get; }

        /// <summary>
        /// Gets the site service.
        /// </summary>
        public ISiteService Sites { get; }

        /// <summary>
        /// Gets the site type service.
        /// </summary>
        public ISiteTypeService SiteTypes { get; }

        /// <summary>
        /// Gets if the current repository has caching enabled or not.
        /// </summary>
        public bool IsCached => _cache != null;

        /// <summary>
        /// Creates a new api from the currently registered
        /// repositories.
        /// </summary>
        public Api(
            ILegacyContentFactory legacyContentFactory,
            IContentFactory contentFactory,
            IAliasRepository aliasRepository,
            IArchiveRepository archiveRepository,
            IContentRepository contentRepository,
            IContentGroupRepository contentGroupRepository,
            IContentTypeRepository contentTypeRepository,
            ILanguageRepository languageRepository,
            IMediaRepository mediaRepository,
            IPageRepository pageRepository,
            IPageTypeRepository pageTypeRepository,
            IParamRepository paramRepository,
            IPostRepository postRepository,
            IPostTypeRepository postTypeRepository,
            ISiteRepository siteRepository,
            ISiteTypeRepository siteTypeRepository,
            ICache cache = null,
            IStorage storage = null,
            IImageProcessor processor = null,
            ISearch search = null)
        {
            // Store the cache
            _cache = cache;

            // Create services without dependecies
            ContentTypes = new ContentTypeService(contentTypeRepository, cache);
            ContentGroups = new ContentGroupService(contentGroupRepository, cache);
            Languages = new LanguageService(languageRepository, cache);
            PageTypes = new PageTypeService(pageTypeRepository, cache);
            Params = new ParamService(paramRepository, cache);
            PostTypes = new PostTypeService(postTypeRepository, cache);
            Sites = new SiteService(siteRepository, legacyContentFactory, cache);
            SiteTypes = new SiteTypeService(siteTypeRepository, cache);

            // Create services with dependencies
            Aliases = new AliasService(aliasRepository, Sites, cache);
            Content = new ContentService(contentRepository, contentFactory, Languages, cache, search);
            Media = new MediaService(mediaRepository, Params, storage, processor, cache);
            Pages = new PageService(pageRepository, legacyContentFactory, Sites, Params, cache, search);
            Posts = new PostService(postRepository, legacyContentFactory, Sites, Pages, Params, Media, cache, search);
            Archives = new ArchiveService(archiveRepository, Params, Posts);
        }

        /// <summary>
        /// Disposes the current api.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
